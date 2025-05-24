namespace KC.Utils.Common.Crypto
{
    public record CryptoOptions(byte[] Key, int KeySize = 256);
}
