namespace KickOff
{
	public interface IStage
	{
		void OnStart(StageArgs args);
		void OnStop(StageArgs args);
	}
}
