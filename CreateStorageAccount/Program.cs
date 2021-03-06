﻿using Microsoft.Azure;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace CreateStorageAccount
{
    class Program
    {
        public static string subscriptionId = "e0fbea86-6cf2-4b2d-81e2-9c59f4f922cb";
        public static string resourceGroupName = "yuvmtest";
        public static string storageAccountName = "yuyutest123";
        public static string clientId = "16a94c84-c6ba-4f19-aa48-3353f8ffe111";
        public static string clientSecret = "123456";


        static void Main(string[] args)
        {
            //构建创建存储所需的参数
            StorageAccountCreateParameters parameters = new StorageAccountCreateParameters(AccountType.StandardGRS,"China North");

            //获取认证token
            var token = GetAccessTokenAsync().Result.AccessToken.ToString();
            var credential = new TokenCloudCredentials(subscriptionId, token);

            StorageManagementClient client = new StorageManagementClient(credential, new Uri("https://management.chinacloudapi.cn"));

            //检查存储名称可用性
            CheckNameAvailabilityResponse nameAvailble = client.StorageAccounts.CheckNameAvailability(storageAccountName);

            if (nameAvailble.NameAvailable)
            {
                //创建存储账户
                client.StorageAccounts.Create(resourceGroupName,storageAccountName,parameters);

                //获取存储秘钥
                StorageAccountListKeysResponse keylists = client.StorageAccounts.ListKeys(resourceGroupName, storageAccountName);
                Console.WriteLine("Key1:"+keylists.StorageAccountKeys.Key1 + "Key2" + keylists.StorageAccountKeys.Key2);

                //获取账户属性
                StorageAccountGetPropertiesResponse properties = client.StorageAccounts.GetProperties(resourceGroupName, storageAccountName);
                Console.WriteLine("Blob Primary endpoint:" + properties.StorageAccount.PrimaryEndpoints.Blob);

                Console.WriteLine("create success");
            }
            else
            {
                Console.WriteLine("改存储账户名已经被其它用户使用，请使用新的存储账户名!");
            }

            Console.ReadKey(true);
        }

        private static async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            var cc = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext("https://login.chinacloudapi.cn/b388b808-0ec9-4a09-a414-a7cbbd8b7e9b");
            var token = await context.AcquireTokenAsync("https://management.chinacloudapi.cn/", cc);
            if (token == null)
            {
                throw new InvalidOperationException("Could not get the token");
            }
            return token;
        }
    }
}
