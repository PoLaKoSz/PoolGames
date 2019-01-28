using System;

namespace CSharpSnookerCore.Models
{

	public interface IRemotingObserver
	{
		void NotifyText(string text);
        void NotifyBallPositionList(int playCount, string ballPositionListText);
        void NotifyPlayerList(string playerListText);
        void NotifyInvitation(string invitation);
	}
}
