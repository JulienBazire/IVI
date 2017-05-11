using System;
using System.IO;
using System.Collections.Generic;

public interface IVIEntry {
	string FullPath { get; }
	void CutTo(IVIDirectory target);
	IVIEntry CopyTo(IVIDirectory target);
}


public class IVIEntry_NameComparer : IComparer<IVIEntry> {
	public int Compare(IVIEntry lhs, IVIEntry rhs) {
		string lname, rname;
		if (lhs is IVIFile)
			lname = (lhs as IVIFile).Systemfile.FullName;
		else
			lname = (lhs as IVIDirectory).Systemdirectory.FullName;
		if (rhs is IVIFile)
			rname = (rhs as IVIFile).Systemfile.FullName;
		else
			rname = (rhs as IVIDirectory).Systemdirectory.FullName;
		return lname.CompareTo (rname);
	}
}

public class IVIEntry_LastAccesTimeComparer : IComparer<IVIEntry> {
	public int Compare(IVIEntry lhs, IVIEntry rhs) {
		DateTime ltime, rtime;
		if (lhs is IVIFile)
			ltime = (lhs as IVIFile).Systemfile.LastAccessTime;
		else
			ltime = (lhs as IVIDirectory).Systemdirectory.LastAccessTime;
		if (rhs is IVIFile)
			rtime = (rhs as IVIFile).Systemfile.LastAccessTime;
		else
			rtime = (rhs as IVIDirectory).Systemdirectory.LastAccessTime;
		return ltime.CompareTo (rtime);
	}
}


public class IVIEntry_SizeComparer : IComparer<IVIEntry> {
	public int Compare(IVIEntry lhs, IVIEntry rhs) {
		long lsize, rsize;
		if (lhs is IVIFile)
			lsize = (lhs as IVIFile).Systemfile.Length;
		else
			lsize = (lhs as IVIDirectory).GetLength();
		if (rhs is IVIFile)
			rsize = (rhs as IVIFile).Systemfile.Length;
		else
			rsize = (rhs as IVIDirectory).GetLength();
		return lsize.CompareTo (rsize);
	}
}