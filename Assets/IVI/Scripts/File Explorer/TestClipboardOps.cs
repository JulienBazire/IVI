using UnityEngine;
using System.Collections;

public class TestClipboardOps : MonoBehaviour
{
	public string CutDirSrcPath , CutDirDstPath ;
	public string CpyDirSrcPath , CpyDirDstPath ;
	public string CutFileSrcPath, CutFileDstPath;
	public string CpyFileSrcPath, CpyFileDstPath;

	protected IVIDirectory CutDirSrc, CutDirDst;
	protected IVIDirectory CpyDirSrc, CpyDirDst;
	protected IVIFile CutFileSrc, CpyFileSrc;
	protected IVIDirectory CutFileDst, CpyFileDst;

	void Start () {
		CutDirSrc = new IVIDirectory (CutDirSrcPath, 0);	
		CutDirDst = new IVIDirectory (CutDirDstPath, 0);
		CpyDirSrc = new IVIDirectory (CpyDirSrcPath, 0);
		CpyDirDst = new IVIDirectory (CpyDirDstPath, 0);
		CutFileSrc = new IVIFile (CutFileSrcPath);
		CpyFileSrc = new IVIFile (CpyFileSrcPath);
		CutFileDst = new IVIDirectory (CutFileDstPath, 0);
		CpyFileDst = new IVIDirectory (CpyFileDstPath, 0);

		CutDirSrc.CutTo (CutDirDst);
		CpyDirSrc.CopyTo (CpyDirDst);
		CutFileSrc.CutTo (CutFileDst);
		CpyFileSrc.CopyTo (CpyFileDst);
	}
	
	void Update () {
	
	}
}

