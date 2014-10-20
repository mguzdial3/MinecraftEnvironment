using System.Collections;

namespace Environment{
	public class OscReceiver {

		public string RemoteIP = "127.0.0.1";
		public int SendToPort = 50001;
		public int ListenerPort= 57131;
		private UDPPacketIO controller; 
		private Osc handler;


		// Use this for initialization
		public OscReceiver () {
			//Initializes on start up to listen for messages
			//make sure this game object has both UDPPackIO and OSC script attached

			controller = new UDPPacketIO ();
			handler = new Osc ();
			controller.init(RemoteIP, SendToPort, ListenerPort);
			handler.init(controller);
				
			handler.SetAddressHandler("/rightarm", Example1);
			handler.SetAddressHandler("/leftarm", Example2);
			handler.SetAddressHandler("/head", Example3);
		}

		
		public void Example1(OscMessage oscMessage){

		}
		
		public void Example2(OscMessage oscMessage){

		}
		
		//HEAD
		public void Example3(OscMessage oscMessage){
		}
		

	}
}
