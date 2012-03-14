public interface iCS_ITreeViewItem {
	string      ToString();
	object		GetParent();
	object[]	GetChildren();
	bool		HasChildren();
	bool		IsFolded();
	void		SetIsFolded(bool folded);
}
