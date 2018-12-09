using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Hondarersoft.Dpm.ServiceProcess
{
    // Is there a way to modify a process DACL in C#
    // https://stackoverflow.com/questions/1909084/is-there-a-way-to-modify-a-process-dacl-in-c-sharp

    public class ServiceSecurity : NativeObjectSecurity
    {
        protected SafeHandle serviceHandle;

        public ServiceSecurity(SafeHandle serviceHandle)
            : base(false, ResourceType.Service, serviceHandle, AccessControlSections.Access)
        {
            this.serviceHandle = serviceHandle;
        }

        public void AddAccessRule(ServiceAccessRule rule)
        {
            base.AddAccessRule(rule);

            // ルールの変更を即時反映する
            Persist(serviceHandle, AccessControlSections.Access);
        }

        public void Persist(SafeHandle serviceHandle)
        {
            Persist(serviceHandle, AccessControlSections.Access);
        }

        public override Type AccessRightType
        {
            get
            {
                return typeof(ServiceAccessRights);
            }
        }

        public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
        {
            return new ServiceAccessRule(identityReference, (ServiceAccessRights)accessMask, type);
        }

        public override Type AccessRuleType
        {
            get
            {
                return typeof(ServiceAccessRule);
            }
        }

        public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
        {
            throw new NotImplementedException();
        }

        public override Type AuditRuleType
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class ServiceAccessRule : AccessRule
    {
        public ServiceAccessRule(IdentityReference identityReference, ServiceAccessRights accessMask, AccessControlType type)
            : base(identityReference, (int)accessMask, false, InheritanceFlags.None, PropagationFlags.None, type)
        {
        }

        public ServiceAccessRule(string identity, ServiceAccessRights accessMask, AccessControlType type)
            : base((SecurityIdentifier)new NTAccount(identity).Translate(typeof(SecurityIdentifier)), (int)accessMask, false, InheritanceFlags.None, PropagationFlags.None, type)
        {
        }

        public ServiceAccessRights ServiceAccessRights
        {
            get
            {
                return (ServiceAccessRights)AccessMask;
            }
        }
    }

    [Flags]
    public enum ServiceAccessRights : uint
    {
        SERVICE_ALL_ACCESS = (0xF01FF),
        SERVICE_CHANGE_CONFIG = (0x0002),
        SERVICE_ENUMERATE_DEPENDENTS = (0x0008),
        SERVICE_INTERROGATE = (0x0080),
        SERVICE_PAUSE_CONTINUE = (0x0040),
        SERVICE_QUERY_CONFIG = (0x0001),
        SERVICE_QUERY_STATUS = (0x0004),
        SERVICE_START = (0x0010),
        SERVICE_STOP = (0x0020),
        SERVICE_USER_DEFINED_CONTROL = (0x0100),

        ACCESS_SYSTEM_SECURITY = 0x01000000,
        DELETE = (0x00010000), // Required to delete the object. 
        READ_CONTROL = (0x00020000), // Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right. 
        WRITE_DAC = (0x00040000), // Required to modify the DACL in the security descriptor for the object. 
        WRITE_OWNER = (0x00080000), // Required to change the owner in the security descriptor for the object. 

        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000,
        GENERIC_EXECUTE = 0x20000000,

        STANDARD_RIGHTS_READ = 0x00020000,
        STANDARD_RIGHTS_WRITE = 0x00020000,
        STANDARD_RIGHTS_EXECUTE = 0x00020000
    }
}
