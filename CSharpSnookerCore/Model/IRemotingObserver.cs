using System;
using System.Collections.Generic;

namespace CSharpSnookerCore.Model
{

	public interface IRemotingObserver
	{
		void NotifyText(string text);
        void NotifyBallPositionList(int playCount, string ballPositionListText);
        void NotifyPlayerList(string playerListText);
        void NotifyInvitation(string invitation);
	}
}
