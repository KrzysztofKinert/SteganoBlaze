using Microsoft.JSInterop;
using SteganoBlaze.Shared;

namespace SteganoBlaze.Utils
{
    public static class JsInterop
    {
        public static async Task<byte[]> ReadStream(IJSRuntime js, string jsFunction)
        {
            try
            {
                IJSStreamReference dataStreamReference = await js.InvokeAsync<IJSStreamReference>(jsFunction);
                var dataBytes = new byte[dataStreamReference.Length];
                await using var stream = await dataStreamReference.OpenReadStreamAsync(AppState.MAX_CARRIER_SIZE * 10);
                await stream.ReadAsync(dataBytes);
                return dataBytes;
            }
            catch
            {
                throw new Exception();
            }
        }
        public static async Task<byte[]> ReadStream(IJSRuntime js, string jsFunction, List<byte[]> aesInputs)
        {
            try
            {
                IJSStreamReference dataStreamReference = await js.InvokeAsync<IJSStreamReference>(jsFunction, aesInputs[0], aesInputs[1], aesInputs[2], aesInputs[3]);
                var dataBytes = new byte[dataStreamReference.Length];
                await using var stream = await dataStreamReference.OpenReadStreamAsync(AppState.MAX_CARRIER_SIZE * 10);
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
