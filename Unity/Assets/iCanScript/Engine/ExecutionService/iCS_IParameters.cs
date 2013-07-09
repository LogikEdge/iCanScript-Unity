// -------------------------------------------------------------------------
// Parameter Protocol
public interface iCS_IParameters {
	string GetParameterName(int idx);
    object GetParameter(int idx);
    void   SetParameter(int idx, object value);
    bool   IsParameterReady(int idx, int frameId);
	void   SetParameterConnection(int idx, iCS_Connection connection);
}
