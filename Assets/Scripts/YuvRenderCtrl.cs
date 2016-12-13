using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;

public class YuvRenderCtrl : MonoBehaviour
{
	Texture2D yTexture;
	Texture2D uvTexture;
	int texWidth = 0;
	int texHeight = 0;
	static UnityTransferMessage mTransMsg;
	static bool isDataChanged = false;

	//与Android jni交互

	struct UnityTransferMessage
	{
		public int degree;
		public int width;
		public int height;
		public IntPtr yPtr;
		public IntPtr uvPtr;

		public bool isNotNull ()
		{
			//need no concern about degree.
			return width != 0 && height != 0 && yPtr != IntPtr.Zero && uvPtr != IntPtr.Zero;
		}
	}


	#if UNITY_ANDROID
	delegate void UnityTransfer (IntPtr msg);

	[DllImport ("unity-transfer-lib")]
	static extern void setTransferByUnity (UnityTransfer transfer);
	#endif

	[MonoPInvokeCallback (typeof(UnityTransfer))]
	static void transformDelegate (IntPtr msg)
	{
		mTransMsg = (UnityTransferMessage)Marshal.PtrToStructure (msg, typeof(UnityTransferMessage));
		isDataChanged = true;
	}

	// Use this for initialization
	void Start ()
	{
		yTexture = transform.GetComponent<MeshRenderer> ().sharedMaterial.GetTexture ("_yTex") as Texture2D;
		uvTexture = transform.GetComponent<MeshRenderer> ().sharedMaterial.GetTexture ("_uvTex") as Texture2D;

		//注册transfer
		setTransferByUnity (transformDelegate);
	}

	// Update is called once per frame
	void Update ()
	{
		//check if need re-upload image data to gpu
		if (isDataChanged && mTransMsg.isNotNull ()) {
			//check if size need to be changed
			if (texWidth != mTransMsg.width || texHeight != mTransMsg.height) {
				texWidth = mTransMsg.width;
				texHeight = mTransMsg.height;

				yTexture.Resize (texWidth, texHeight, TextureFormat.Alpha8, false);
				uvTexture.Resize (texWidth / 2, texHeight / 2, TextureFormat.RGB24, false);
			}
			//load data to texture cpu memery
			yTexture.LoadRawTextureData (mTransMsg.yPtr, texWidth * texHeight);
			uvTexture.LoadRawTextureData (mTransMsg.uvPtr, texWidth * texHeight * 3 / 4);
			//upload data from cpu memery to gpu
			yTexture.Apply ();
			uvTexture.Apply ();
			//mark data has completely changed,and need no re-upload
			isDataChanged = false;
		}
	}

	void OnDestroy ()
	{
		yTexture = null;
		uvTexture = null;
	}
}
