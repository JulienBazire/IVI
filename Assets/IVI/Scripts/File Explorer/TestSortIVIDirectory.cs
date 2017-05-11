using UnityEngine;
using System.Collections.Generic;

public class TestSortIVIDirectory : MonoBehaviour
{
	static protected readonly string ROOT = "D:\\Yoon\\Documents";
	protected IVIDirectory dir = new IVIDirectory(ROOT, 2);

	void Start () {
		SortAndLog(new IVIEntry_NameComparer());
		SortAndLog(new IVIEntry_LastAccesTimeComparer());
		SortAndLog(new IVIEntry_SizeComparer());
	}
	void SortAndLog(IComparer<IVIEntry> cmp) {
		dir.Entries.Sort (cmp);
		Debug.Log("Sorted by " + cmp.GetType().FullName + " ...");
		LogDentries();
	}
	void LogDentries() {
		foreach (IVIEntry entry in dir.Entries) {
			if (entry is IVIDirectory) {
				IVIDirectory edir = entry as IVIDirectory;
				Debug.Log (edir.Systemdirectory.FullName);
			} else {
				IVIFile efile = entry as IVIFile;
				Debug.Log (efile.Systemfile.FullName);
			}
		}
	}
}

