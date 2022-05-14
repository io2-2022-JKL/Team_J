namespace IdentityServer.Models
{
    public static class Role
    {
        public const string Admin = "admin";
        public const string Doctor = "doctor";
        public const string Patient = "patient";
        public static string[] Roles = { Admin, Doctor, Patient };

        public static bool IsRole(string name)
        {
            foreach(string role in Roles)
            {
                if (role == name)
                    return true;
            }
            return false;
        }

    }
}
