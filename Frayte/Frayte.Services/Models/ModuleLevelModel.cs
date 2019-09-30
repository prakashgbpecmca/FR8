using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class AccessRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameDisplay { get; set; } 
    }
    public class AccessModule
    {
        public int ModuleLevelId { get; set; }
        public string ModuleName { get; set; }
        public string MultilingualKey { get; set; }
        public int OrderNumber { get; set; }
        public List<AccessSubModule> AccessSubModules { get; set; }
    }
    public class AccessSubModule
    {
        public int ModuleLevelDetailId { get; set; }
        public int ModuleLevelId { get; set; }
        public string MultilingualKey { get; set; }
        public string SubModuleName { get; set; }
        public string Route { get; set; }
        public int OrderNumber { get; set; }
    }

    public class AccessRoleModule
    {
        public int RoleModuleId { get; set; }
        public int RoleId { get; set; }
        public int ModuleLeveLId { get; set; }
        public int ModuleLeveLDetailId { get; set; }
        public string Route { get; set; }
        public string UpdateCase { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }

        public List<AccessRoleModule> MyProperty { get; set; }
        public int OrderNumber { get; set; }
    }



    public class AccessModuleLevel
    {
        public int RoleId { get; set; }
        public int ModuleLevelId { get; set; }
    }

    public class ChildModuleRole
    {
        public int ModuleLevelId { get; set; }
        public int ModuleLevelDetailId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleNameDisplay { get; set; }
        public string MultilingualKey { get; set; }
        public List<AccessRoleModule> RoleModules { get; set; }
    }

    public class ModuleRole
    {
        public int ModuleLevelId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleNameDisplay { get; set; }
        public bool ShowHideModule { get; set; }
        public List<AccessRoleModule> RoleModules { get; set; }
        public List<ChildModuleRole> ChildRoleModules { get; set; }
    
    }
    public class AccessLevel
    {
        public List<AccessRole> Roles { get; set; }

        public List<ModuleRole> ModuleRoles { get; set; }

    }
}
