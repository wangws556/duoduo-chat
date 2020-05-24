package
{
	import flash.net.NetConnection;
	import flash.net.NetStream;
	
	public class Receiver extends Session
	{
		override public function PublishVideo(idx:int,w:int, h:int, f:uint, q:uint)
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting publish video...");
				netStream.receiveVideo(true);
				netStream.play(sessionId);
				videoState.State = SessionState.Normal;
				callCSharp("flashLogger","Started publish video");
				return netStream;
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
			return null;
		}
		
		
		override public function PublishAudio(idx:int, silence:uint = 0, gain:uint = 50, rate:uint = 8, volume:Number = 1)
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting publish audio...");
				netStream.receiveAudio(true);
				netStream.play(sessionId);
				audioState.State = SessionState.Normal;
				callCSharp("flashLogger","Started publish audio");
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
		
		override public function PauseAudio()
		{
			if(initialized)
			{
				if(audioState.State  == SessionState.Normal ||
					audioState.State == SessionState.Resumed)
				{
					callCSharp("flashLogger","Starting pause audio...");
					netStream.receiveAudio(false);
					netStream.play(sessionId);
					audioState.State = SessionState.Paused;
					callCSharp("flashLogger","Started pause audio");
				}
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
		
		override public function ResumeAudio()
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting resume audio...");
				if(audioState.State == SessionState.Paused)
				{
					netStream.receiveAudio(true);
					netStream.play(sessionId);
					audioState.State = SessionState.Resumed;
				}
				else if(audioState.State == SessionState.None)
				{
					PublishAudio(microphoneIndex);
				}
				callCSharp("flashLogger","Started resume audio");
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
		
		override public function Destroy()
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting destroy...");
				try
				{
					netStream.receiveAudio(false);
					netStream.receiveVideo(false);
					netStream.close();
				}
				catch(e:Error){
					callCSharp("flashLogger",e.message);
				}
				netStream = null;	
				initialized = false;
				callCSharp("flashLogger","Started destroy");
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
		override public function PauseVideo()
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting pause video...");
				if(videoState.State == SessionState.Normal ||
					videoState.State == SessionState.Resumed)
				{
					netStream.receiveVideo(false);
					netStream.play(sessionId);
					videoState.State = SessionState.Paused;
				}
				callCSharp("flashLogger","Started pause video");
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
		override public function ResumeVideo()
		{
			if(initialized)
			{
				callCSharp("flashLogger","Starting resume video...");
				if(videoState.State == SessionState.Paused)
				{
					netStream.receiveVideo(true);
					netStream.play(sessionId);
					videoState.State = SessionState.Resumed;
				}
				else if(videoState.State == SessionState.None)
				{
					PublishVideo(cameraIndex,width,height,fps,quality);
				}
				callCSharp("flashLogger","Started resume video");
				return netStream;
			}
			else{
				callCSharp("flashLogger","Not initialzied, cannot publish audio");
			}
		}
	}
}