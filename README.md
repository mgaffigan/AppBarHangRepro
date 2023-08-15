# Example showing .Net System Events hang

Necessary preconditions:

1. a change to desktop layout must come as the Application is shutting down
2. (To cause `SystemEvents.UserPreferenceChanging` to be created via `VisualStyleRenderer..cctor`) the form must have at least one control

Dump of hung process: [WinFormsSystemEventsHangRepro.dmp.zip](WinFormsSystemEventsHangRepro.dmp.zip)

Notes from hung process:

- `Program.Main` has exited.  Application has shutdown.  Only non-user threads remain.
- Apparent "forever wait" by a thread named ".NET System Events" on a wait handle (see call stack below)
- Apparent "Join" on worker thread with "SystemEvents.Shutdown" of the ".NET System Events" thread

### Main Thread

	win32u.dll!NtUserMsgWaitForMultipleObjectsEx()
	combase.dll!CCliModalLoop::BlockFn(void * * ahEvent, unsigned long cEvents, unsigned long * lpdwSignaled) Line 2108
		at onecore\com\combase\dcomrem\callctrl.cxx(2108)
	combase.dll!ClassicSTAThreadWaitForHandles(unsigned long dwFlags, unsigned long dwTimeout, unsigned long cHandles, void * * pHandles, unsigned long * pdwIndex) Line 54
		at onecore\com\combase\dcomrem\classicsta.cpp(54)
	combase.dll!CoWaitForMultipleHandles(unsigned long dwFlags, unsigned long dwTimeout, unsigned long cHandles, void * * pHandles, unsigned long * lpdwindex) Line 126
		at onecore\com\combase\dcomrem\sync.cxx(126)
	[Inline Frame] hostpolicy.dll!coreclr_t::shutdown(int *) Line 152
		at D:\a\_work\1\s\src\native\corehost\hostpolicy\coreclr.cpp(152)
	hostpolicy.dll!run_app_for_context(const hostpolicy_context_t & context, int argc, const wchar_t * * argv) Line 264
		at D:\a\_work\1\s\src\native\corehost\hostpolicy\hostpolicy.cpp(264)
	hostpolicy.dll!run_app(const int argc, const wchar_t * * argv) Line 284
		at D:\a\_work\1\s\src\native\corehost\hostpolicy\hostpolicy.cpp(284)
	hostpolicy.dll!corehost_main(const int argc, const wchar_t * * argv) Line 430
		at D:\a\_work\1\s\src\native\corehost\hostpolicy\hostpolicy.cpp(430)
	hostfxr.dll!execute_app(const std::wstring & impl_dll_dir, corehost_init_t * init, const int argc, const wchar_t * * argv) Line 146
		at D:\a\_work\1\s\src\native\corehost\fxr\fx_muxer.cpp(146)
	hostfxr.dll!`anonymous namespace'::read_config_and_execute(const std::wstring & host_command, const host_startup_info_t & host_info, const std::wstring & app_candidate, const std::unordered_map<enum known_options,std::vector<std::wstring,std::allocator<std::wstring>>,known_options_hash,std::equal_to<enum known_options>,std::allocator<std::pair<enum known_options const ,std::vector<std::wstring,std::allocator<std::wstring>>>>> & opts, int new_argc, const wchar_t * * new_argv, host_mode_t mode, const bool is_sdk_command, wchar_t * out_buffer, int buffer_size, int * required_buffer_size) Line 533
		at D:\a\_work\1\s\src\native\corehost\fxr\fx_muxer.cpp(533)
	hostfxr.dll!fx_muxer_t::handle_exec_host_command(const std::wstring & host_command, const host_startup_info_t & host_info, const std::wstring & app_candidate, const std::unordered_map<enum known_options,std::vector<std::wstring,std::allocator<std::wstring>>,known_options_hash,std::equal_to<enum known_options>,std::allocator<std::pair<enum known_options const ,std::vector<std::wstring,std::allocator<std::wstring>>>>> & opts, int argc, const wchar_t * * argv, int argoff, host_mode_t mode, const bool is_sdk_command, wchar_t * result_buffer, int buffer_size, int * required_buffer_size) Line 1018
		at D:\a\_work\1\s\src\native\corehost\fxr\fx_muxer.cpp(1018)
	hostfxr.dll!fx_muxer_t::execute(const std::wstring host_command, const int argc, const wchar_t * * argv, const host_startup_info_t & host_info, wchar_t * result_buffer, int buffer_size, int * required_buffer_size) Line 579
		at D:\a\_work\1\s\src\native\corehost\fxr\fx_muxer.cpp(579)
	hostfxr.dll!hostfxr_main_startupinfo(const int argc, const wchar_t * * argv, const wchar_t * host_path, const wchar_t * dotnet_root, const wchar_t * app_path) Line 61
		at D:\a\_work\1\s\src\native\corehost\fxr\hostfxr.cpp(61)
	WinFormsSystemEventsHangRepro.exe!00007ff71dc92a80()
	WinFormsSystemEventsHangRepro.exe!00007ff71dc92dfb()
	WinFormsSystemEventsHangRepro.exe!00007ff71dc942a8()
	kernel32.dll!00007ffa54e126ad()
	ntdll.dll!00007ffa563eaa68()

### .NET System Events call stack

	System.Private.CoreLib.dll!System.Threading.WaitHandle.WaitOneNoCheck(int millisecondsTimeout) Line 139
		at /_/src/libraries/System.Private.CoreLib/src/System/Threading/WaitHandle.cs(139)
	System.Windows.Forms.dll!System.Windows.Forms.Control.WaitForWaitHandle(System.Threading.WaitHandle waitHandle) Line 3967
		at /_/src/System.Windows.Forms/src/System/Windows/Forms/Control.cs(3967)
	System.Windows.Forms.dll!System.Windows.Forms.Control.MarshaledInvoke(System.Windows.Forms.Control caller, System.Delegate method, object[] args, bool synchronous) Line 7141
		at /_/src/System.Windows.Forms/src/System/Windows/Forms/Control.cs(7141)
	System.Windows.Forms.dll!System.Windows.Forms.Control.Invoke(System.Delegate method, object[] args) Line 6587
		at /_/src/System.Windows.Forms/src/System/Windows/Forms/Control.cs(6587)
	System.Windows.Forms.dll!System.Windows.Forms.WindowsFormsSynchronizationContext.Send(System.Threading.SendOrPostCallback d, object state) Line 88
		at /_/src/System.Windows.Forms/src/System/Windows/Forms/WindowsFormsSynchronizationContext.cs(88)
	Microsoft.Win32.SystemEvents.dll!Microsoft.Win32.SystemEvents.SystemEventInvokeInfo.Invoke(bool checkFinalization, object[] args) Line 35
		at Microsoft.Win32\SystemEvents.cs(35)
	Microsoft.Win32.SystemEvents.dll!Microsoft.Win32.SystemEvents.RaiseEvent(bool checkFinalization, object key, object[] args) Line 850
		at Microsoft.Win32\SystemEvents.cs(850)
		locals:
			array[0] 
				._delegate = System.Windows.Forms.VisualStyles.VisualStyleRenderer.OnUserPreferenceChanging
				._syncCtx = System.Windows.Forms.WindowsFormsSynchronizationContext
	Microsoft.Win32.SystemEvents.dll!Microsoft.Win32.SystemEvents.WindowProc(nint hWnd, int msg, nint wParam, nint lParam) Line 961
		at Microsoft.Win32\SystemEvents.cs(961)
		locals:
			msg = 8218
			wParam = 0x2f
			lParam = 0
	[Native to Managed Transition]
	[Managed to Native Transition]
	Microsoft.Win32.SystemEvents.dll!Interop.User32.DispatchMessageW.____PInvoke|210_0(Interop.User32.MSG* msg)
	Microsoft.Win32.SystemEvents.dll!Microsoft.Win32.SystemEvents.WindowThreadProc() Line 1038
		at Microsoft.Win32\SystemEvents.cs(1038)

### Background shutdown thread

	System.Private.CoreLib.dll!System.Threading.Thread.Join() Line 547
		at /_/src/libraries/System.Private.CoreLib/src/System/Threading/Thread.cs(547)
	Microsoft.Win32.SystemEvents.dll!Microsoft.Win32.SystemEvents.Shutdown() Line 907
		at Microsoft.Win32\SystemEvents.cs(907)

## Workaround

Use a custom `ApplicationContext` which ensures the `SystemEvents` thread is not currently hung before exiting

    internal class SafeApplicationContext : ApplicationContext
    {
        public SafeApplicationContext(Form mainForm)
            : base(mainForm)
        {
        }

        [DllImport("user32.dll")]
        static extern void PostQuitMessage(int nExitCode);

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            var syncCtx = SynchronizationContext.Current;
            SystemEvents.InvokeOnEventsThread(() =>
            {
                PostQuitMessage(0);
                syncCtx.Post((_) =>
                {
                    ExitThread();
                }, null);
            });
        }
    }
