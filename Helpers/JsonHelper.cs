using System.Text.Json;
namespace FabstoreWebApplication.Helpers
    {
    public sealed class JsonHelper
        {
        public static string AsJsonString(object obj)
            {
            try
                {
                return JsonSerializer.Serialize(obj);
                }
            catch (Exception ex)
                {
                return $"Exception Occurred: {ex}";
                }

            }
        }
    }
