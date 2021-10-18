/**
 * Generate from swagger json url: http://localhost:56876/swagger/v1/swagger.json
 **/

import { request } from "umi";

/**
 * api count: 122
 **/

/**
 * *TODO*  get /api/abp/api-definition
 **/
export async function GetAbpApiDefinition(
    params: { IncludeTypes },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/abp/api-definition`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/abp/application-configuration
 **/
export async function GetAbpApplicationConfiguration(options?: {
    [key: string]: any;
}) {
    return request<API.IdentityRoleDto>(`/api/abp/application-configuration`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/register
 **/
export async function Register(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/register`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/send-password-reset-code
 **/
export async function SendPasswordResetCode(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(
        `/api/account/send-password-reset-code`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/account/reset-password
 **/
export async function ResetPassword(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/reset-password`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/application-configuration
 **/
export async function GetApplicationConfiguration(options?: {
    [key: string]: any;
}) {
    return request<API.IdentityRoleDto>(`/api/application-configuration`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/application-configuration/admin
 **/
export async function GetAdminApplicationConfiguration(options?: {
    [key: string]: any;
}) {
    return request<API.IdentityRoleDto>(
        `/api/application-configuration/admin`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/clouds/api-account
 **/
export async function CreateCloudApiAccount(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/clouds/api-account`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/api-account
 **/
export async function GetCloudApiAccountList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/api-account`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/clouds/api-account/{id}
 **/
export async function DeleteCloudApiAccount(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/api-account/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/api-account/{id}
 **/
export async function GetCloudApiAccount(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/api-account/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/clouds/api-account/{id}
 **/
export async function UpdateCloudApiAccount(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/api-account/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/providers
 **/
export async function GetAllCloudProvider(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/clouds/providers`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/clouds/regions
 **/
export async function CreateCloudRegion(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/regions
 **/
export async function GetCloudRegionList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/clouds/regions/{id}
 **/
export async function DeleteCloudRegion(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/regions/{id}
 **/
export async function GetCloudRegion(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/clouds/regions/{id}
 **/
export async function UpdateCloudRegion(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/clouds/regions/all
 **/
export async function GetAllCloudRegion(
    params: { provider },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/clouds/regions/all`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/disks
 **/
export async function CreateDisk(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/disks
 **/
export async function GetDiskList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/disks`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/disks
 **/
export async function BatchDelete(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/disks/{id}
 **/
export async function DeleteDisk(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/disks/{id}
 **/
export async function GetDisk(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/disks/{id}
 **/
export async function UpdateDisk(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/disks/order
 **/
export async function CreateOrderDisk(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks/order`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/disks/{id}/resize
 **/
export async function Resize(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/disks/${id}/resize`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/flavors/all
 **/
export async function GetAllFlavor(
    params: { Category; RegionId; RegionZoneId },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/flavors/all`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/identity/change-email
 **/
export async function ChangeEmail(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/change-email`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/identity/change-phone-number
 **/
export async function ChangePhoneNumber(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/change-phone-number`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/images/all
 **/
export async function GetAllImage(
    params: { ProviderId },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/images/all`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/forget-password
 **/
export async function ForgetPassword(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/forget-password`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/user/login
 **/
export async function UserLogin(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/user/login`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/account/logout
 **/
export async function Logout(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/logout`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/user/register
 **/
export async function Register(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/user/register`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/account/admin/login
 **/
export async function AdminLogin(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/account/admin/login`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/account/{id}/proxy-login
 **/
export async function GetProxyLoginMyAccount(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/account/${id}/proxy-login`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/account/proxy-login
 **/
export async function UserProxyLogin2(
    params: { Token },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/account/proxy-login`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/networks/regions/{regionId}/default
 **/
export async function GetDefaultNetwork(
    regionId: string,
    params: { providerId },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/networks/regions/${regionId}/default`,
        {
            method: "get",
            params: params,
            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/physical-machine-applies
 **/
export async function CreatePhysicalMachineApply(options?: {
    [key: string]: any;
}) {
    return request<API.IdentityRoleDto>(`/api/physical-machine-applies`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/physical-machine-applies
 **/
export async function GetPhysicalMachineApplyList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/physical-machine-applies`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/physical-machine-applies/{id}
 **/
export async function DeletePhysicalMachineApply(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/physical-machine-applies/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/physical-machine-applies/{id}
 **/
export async function GetPhysicalMachineApply(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/physical-machine-applies/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/physical-machine-applies/{id}
 **/
export async function UpdatePhysicalMachineApply(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/physical-machine-applies/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/PrivateMessage/{id}
 **/
export async function GetPrivateMessage(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/PrivateMessage/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/PrivateMessage
 **/
export async function GetPrivateMessageList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/PrivateMessage`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/PrivateMessage/make-all-read
 **/
export async function MakeAllAsRead(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/PrivateMessage/make-all-read`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/products
 **/
export async function CreateProduct(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/products`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/products
 **/
export async function GetProductList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/products`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/products/{id}
 **/
export async function DeleteProduct(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/products/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/products/{id}
 **/
export async function GetProduct(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/products/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/products/{id}
 **/
export async function UpdateProduct(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/products/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/my-profile
 **/
export async function GetProfile(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/my-profile`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/identity/my-profile
 **/
export async function UpdateProfile(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/my-profile`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/identity/my-profile/change-password
 **/
export async function ChangePassword(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(
        `/api/identity/my-profile/change-password`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  delete /api/networks/eip
 **/
export async function BatchDelete(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/networks/eip`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/networks/eip
 **/
export async function CreatePublicIpaddress(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/networks/eip`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/networks/eip
 **/
export async function GetPublicIpaddressList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/networks/eip`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/networks/eip/{id}
 **/
export async function DeletePublicIpaddress(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/networks/eip/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/networks/eip/{id}
 **/
export async function GetPublicIpaddress(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/networks/eip/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/networks/eip/{id}
 **/
export async function UpdatePublicIpaddress(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/networks/eip/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/roles/all
 **/
export async function GetAllListRole(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/roles/all`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/roles
 **/
export async function GetRoleList(
    params: { Filter; Sorting; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/roles`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/identity/roles
 **/
export async function CreateRole(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/roles`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/roles/{id}
 **/
export async function GetRole(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/roles/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/identity/roles/{id}
 **/
export async function UpdateRole(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/roles/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/identity/roles/{id}
 **/
export async function DeleteRole(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/roles/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/security-groups
 **/
export async function CreateSecurityGroup(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/security-groups`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/security-groups
 **/
export async function GetSecurityGroupList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/security-groups/{id}/rules
 **/
export async function CreateRuleSecurityGroup(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups/${id}/rules`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/security-groups/{id}/rules
 **/
export async function GetRulesSecurityGroup(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups/${id}/rules`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/security-groups/{id}
 **/
export async function DeleteSecurityGroup(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/security-groups/{id}
 **/
export async function GetSecurityGroup(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/security-groups/{id}
 **/
export async function UpdateSecurityGroup(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/security-groups/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/security-groups/{groupId}/rules/{id}
 **/
export async function DeleteRuleSecurityGroup(
    groupId: string,
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/security-groups/${groupId}/rules/${id}`,
        {
            method: "delete",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  put /api/security-groups/{groupId}/rules/{id}
 **/
export async function UpdateRuleSecurityGroup(
    groupId: string,
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/security-groups/${groupId}/rules/${id}`,
        {
            method: "put",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/snapshots/disks/{diskId}
 **/
export async function CreateSnapshot(
    diskId: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/snapshots/disks/${diskId}`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/snapshots/{id}
 **/
export async function DeleteSnapshot(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/snapshots/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/snapshots/{id}
 **/
export async function UpdateSnapshot(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/snapshots/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/snapshots/{id}
 **/
export async function GetSnapshot(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/snapshots/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/snapshots
 **/
export async function GetSnapshotList(
    params: { DiskId; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/snapshots`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/snapshots
 **/
export async function BatchDelete(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/snapshots`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/snapshots/{id}/rollback
 **/
export async function Rollback(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/snapshots/${id}/rollback`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/transactions
 **/
export async function GetTransactionList(
    params: { Type; Status; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/transactions`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/transactions/users/{id}
 **/
export async function GetListByUserIdTransaction(
    id: string,
    params: { Type; Status; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/transactions/users/${id}`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/users
 **/
export async function CreateUser(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/users`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/users
 **/
export async function GetUserList(
    params: { Filter; UserType; IncludeWallet; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/users`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/users/{id}
 **/
export async function DeleteUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/users/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/users/{id}
 **/
export async function GetUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/users/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/users/{id}
 **/
export async function UpdateUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/users/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users/{id}
 **/
export async function GetUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/users/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/identity/users/{id}
 **/
export async function UpdateUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/users/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  delete /api/identity/users/{id}
 **/
export async function DeleteUser(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/users/${id}`, {
        method: "delete",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users
 **/
export async function GetUserList(
    params: { Filter; Sorting; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/users`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/identity/users
 **/
export async function CreateUser(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/users`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users/{id}/roles
 **/
export async function GetRolesUser(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/users/${id}/roles`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/identity/users/{id}/roles
 **/
export async function UpdateRolesUser(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/users/${id}/roles`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users/assignable-roles
 **/
export async function GetAssignableRolesUser(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(
        `/api/identity/users/assignable-roles`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/identity/users/by-username/{userName}
 **/
export async function FindByUsername(
    userName: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/identity/users/by-username/${userName}`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/identity/users/by-email/{email}
 **/
export async function FindByEmail(
    email: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/identity/users/by-email/${email}`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/identity/users/lookup/{id}
 **/
export async function FindById(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/identity/users/lookup/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users/lookup/by-username/{userName}
 **/
export async function FindByUserName(
    userName: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/identity/users/lookup/by-username/${userName}`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/identity/users/lookup/search
 **/
export async function Search(
    params: { Filter; Sorting; SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/users/lookup/search`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/identity/users/lookup/count
 **/
export async function GetCountUserLookup(
    params: { Filter },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/identity/users/lookup/count`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/virtual-machines/action
 **/
export async function BatchAction(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/action`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/virtual-machines/{id}/change-os
 **/
export async function ChangeOs(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/${id}/change-os`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  delete /api/virtual-machines/{id}
 **/
export async function DeleteVirtualMachine(
    id: string,
    params: { PublicIp; DataDisk },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/${id}`, {
        method: "delete",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/virtual-machines/{id}
 **/
export async function GetVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/${id}`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  put /api/virtual-machines/{id}
 **/
export async function UpdateVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/${id}`, {
        method: "put",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/virtual-machines/{id}/bandwidths
 **/
export async function GetBandwidthListVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/${id}/bandwidths`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/virtual-machines/{id}/disks
 **/
export async function GetDiskListVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/${id}/disks`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/virtual-machines
 **/
export async function GetVirtualMachineList(
    params: { SkipCount; MaxResultCount },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines`, {
        method: "get",
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO*  get /api/virtual-machines/{id}/network-interfaces
 **/
export async function GetNetworkInterfaceListVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/${id}/network-interfaces`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/virtual-machines/{id}/remote-console
 **/
export async function GetRemoteControlInfoVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/${id}/remote-console`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/virtual-machines/{id}/security-groups
 **/
export async function GetSecurityGroupListVirtualMachine(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/${id}/security-groups`,
        {
            method: "get",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/virtual-machines/order
 **/
export async function Order(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/order`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/virtual-machines/renewal
 **/
export async function Renewal(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/renewal`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/virtual-machines/reset-password
 **/
export async function ResetAdminPassword(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(
        `/api/virtual-machines/reset-password`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  post /api/virtual-machines/{id}/resize
 **/
export async function Resize(id: string, options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/virtual-machines/${id}/resize`, {
        method: "post",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/wallet/admin/transactions/users/{id}
 **/
export async function AdminApplyTransactions(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleDto>(
        `/api/wallet/admin/transactions/users/${id}`,
        {
            method: "post",

            ...(options || {}),
        }
    );
}

/**
 * *TODO*  get /api/wallet
 **/
export async function GetWallet(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/wallet`, {
        method: "get",

        ...(options || {}),
    });
}

/**
 * *TODO*  post /api/wallet/recharge
 **/
export async function UserRecharge(options?: { [key: string]: any }) {
    return request<API.IdentityRoleDto>(`/api/wallet/recharge`, {
        method: "post",

        ...(options || {}),
    });
}
