using System.Net.WebSockets;
using VTube.DataModel;

namespace VTube
{
    static class Api
    {
        public static string RequestAuthenticationToken(ClientWebSocket clientWebSocket)
        {
            Utils.SendRequest(clientWebSocket, new AuthenticationTokenRequest());
            AuthenticationTokenResponse res = Utils.ReceiveResponse<AuthenticationTokenResponse>(clientWebSocket);
            string authtoken = res.Data.AuthenticationToken;
            return authtoken;
        }

        public static bool RequestAuthentication(ClientWebSocket clientWebSocket, string authtoken)
        {
            Utils.SendRequest(clientWebSocket, new AuthenticationRequest(authtoken));
            AuthenticationResponse res = Utils.ReceiveResponse<AuthenticationResponse>(clientWebSocket);
            bool authenticated = res.Data.Authenticated;
            return authenticated;
        }

        public static InputParameterListResponse.DataSection RequestInputParameterList(ClientWebSocket clientWebSocket)
        {
            Utils.SendRequest(clientWebSocket, new InputParameterListRequest());
            InputParameterListResponse res = Utils.ReceiveResponse<InputParameterListResponse>(clientWebSocket);
            InputParameterListResponse.DataSection data = res.Data;
            return data;
        }

        public static void RequestParameterCreation(ClientWebSocket clientWebSocket, string parameterName, string explanation, float min, float max, float defaultValue)
        {
            Utils.SendRequest(clientWebSocket, new ParameterCreationRequest(parameterName, explanation, min, max, defaultValue));
            ParameterCreationResponse _ = Utils.ReceiveResponse<ParameterCreationResponse>(clientWebSocket);
        }

        public static void RequestInjectParameterData(ClientWebSocket clientWebSocket, bool faceFound, string mode, List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues)
        {
            Utils.SendRequest(clientWebSocket, new InjectParameterDataRequest(faceFound, mode, parameterValues));
            ParameterCreationResponse _ = Utils.ReceiveResponse<ParameterCreationResponse>(clientWebSocket);
        }
    }
}
