using Microsoft.JSInterop;
namespace SteganoBlaze.Shared.Classes
{
	public static class JSInterop
	{
        public static readonly int MAX_CARRIER_SIZE = 10000000;
        public static readonly int MAX_MESSAGE_SIZE = 5000000;
        public static readonly int MAX_CARRIER_PIXELS = 25000000;

        public async static Task<byte[]> ReadStream(IJSRuntime js, string jsFunction, List<byte[]>? aesInputs = null)
        {
            try
            {
                IJSStreamReference dataStreamReference = aesInputs is null ? await js.InvokeAsync<IJSStreamReference>(jsFunction)
                                                                           : await js.InvokeAsync<IJSStreamReference>(jsFunction, aesInputs[0], aesInputs[1], aesInputs[2], aesInputs[3]);

                var dataBytes = new byte[dataStreamReference.Length];
                using (var stream = await dataStreamReference.OpenReadStreamAsync(MAX_CARRIER_SIZE * 10))
                    await stream.ReadAsync(dataBytes);
                return dataBytes;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
