using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Applet.Nat.Ux.Models
{
    public class FileManagerInterop
    {
        private readonly IJSRuntime _jsRuntime;
        public string ivstrFolderPath { get; set; }
        public FileManagerInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SelectFolder()
        {
            await _jsRuntime.InvokeVoidAsync("fileManager.selectFolder", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task FolderSelected(string folderPath)
        {
            ivstrFolderPath=folderPath;
        }
    }
}
