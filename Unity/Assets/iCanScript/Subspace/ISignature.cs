using Subspace;

// -------------------------------------------------------------------------
// Parameter Protocol
public interface ISignature {
    SignatureDataSource GetSignatureDataSource();
    SSAction            GetAction();
}
