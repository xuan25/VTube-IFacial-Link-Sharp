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
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
            string authtoken = res.Data.AuthenticationToken;
            return authtoken;
        }

        public static void RequestAuthentication(ClientWebSocket clientWebSocket, string authtoken)
        {
            Utils.SendRequest(clientWebSocket, new AuthenticationRequest(authtoken));
            AuthenticationResponse res = Utils.ReceiveResponse<AuthenticationResponse>(clientWebSocket);
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
            if (!res.Data.Authenticated)
            {
                throw new Exception($"Authentication Rejected: {res.Data.Reason}");
            }
        }

        public static InputParameterListResponse.DataSection RequestInputParameterList(ClientWebSocket clientWebSocket)
        {
            Utils.SendRequest(clientWebSocket, new InputParameterListRequest());
            InputParameterListResponse res = Utils.ReceiveResponse<InputParameterListResponse>(clientWebSocket);
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
            InputParameterListResponse.DataSection data = res.Data;
            return data;
        }

        public static void RequestParameterCreation(ClientWebSocket clientWebSocket, string parameterName, string explanation, float min, float max, float defaultValue)
        {
            Utils.SendRequest(clientWebSocket, new ParameterCreationRequest(parameterName, explanation, min, max, defaultValue));
            ParameterCreationResponse res = Utils.ReceiveResponse<ParameterCreationResponse>(clientWebSocket);
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
        }

        public static void RequestParameterDeletion(ClientWebSocket clientWebSocket, string parameterName)
        {
            Utils.SendRequest(clientWebSocket, new ParameterDeletionRequest(parameterName));
            ParameterDeletionResponse res = Utils.ReceiveResponse<ParameterDeletionResponse>(clientWebSocket);
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
        }

        public static void RequestInjectParameterData(ClientWebSocket clientWebSocket, bool faceFound, string mode, List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues)
        {
            Utils.SendRequest(clientWebSocket, new InjectParameterDataRequest(faceFound, mode, parameterValues));
            ParameterCreationResponse res = Utils.ReceiveResponse<ParameterCreationResponse>(clientWebSocket);
            if (res.Data.ErrorID != 0)
            {
                throw new Exception($"{res.Data.Message} ({res.Data.ErrorID})");
            }
        }
    }
}
