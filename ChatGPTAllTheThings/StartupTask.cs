using MistyRobotics.Common.Types;
using MistyRobotics.SDK.Messengers;
using Windows.ApplicationModel.Background;

namespace ChatGPTAllTheThings
{
	public sealed class StartupTask : IBackgroundTask
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			RobotMessenger.LoadAndPrepareSkill(taskInstance, new MistySkill(), SkillLogLevel.Verbose);
		}
	}
}