using System;
using System.Collections.Generic;
using System.Threading;
using MistyRobotics.Common.Data;
using MistyRobotics.Common.Types;
using MistyRobotics.SDK;
using MistyRobotics.SDK.Messengers;

namespace ChatGPTAllTheThings
{
	internal class MistySkill : IMistySkill
	{
		private IRobotMessenger _misty;
		private Thread _thread;

		public INativeRobotSkill Skill { get; private set; } = new NativeRobotSkill("ChatGPTAllTheThings", "c6d73174-424d-4f36-a404-50102735dca3") { TimeoutInSeconds = -1 };

		public void LoadRobotConnection(IRobotMessenger robotInterface)
		{
			_misty = robotInterface;
		}

		public async void OnStart(object sender, IDictionary<string, object> parameters)
		{
            await _misty.StopDialogAsync();
			ThreadedSkill threadedSkill = new ThreadedSkill(_misty);
            // I generally run my skill in a seperate thread to avoid accidentially blocking callbacks
            _thread = new Thread(new ThreadStart(threadedSkill.Start));
            _thread.Start();
        }

		public void OnPause(object sender, IDictionary<string, object> parameters)
		{

		}

		public   void OnResume(object sender, IDictionary<string, object> parameters)
		{
		

		}

		public void OnCancel(object sender, IDictionary<string, object> parameters)
		{

		}

		public void OnTimeout(object sender, IDictionary<string, object> parameters)
		{

		}

		#region IDisposable Support
		private bool _isDisposed = false;

		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_isDisposed = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MistySkill() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
