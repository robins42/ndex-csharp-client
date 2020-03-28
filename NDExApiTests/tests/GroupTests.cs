using System;
using System.Collections.Generic;
using System.Net;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;

namespace NDExApiTests.tests
{
    public class GroupTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetGroupWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .Get(Guid.Empty));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Group with UUID: " + Guid.Empty + " doesn't exist.", exception.Message);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetGroup(RestImplementation restImpl)
        {
            Group group = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Get(SharedIds.GroupId1);

            Assert.NotNull(group);
            Assert.Equal(SharedIds.GroupId1, group.externalId);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void CreateGroupError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .Create(null));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Request body cannot be null.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void CreateGroup(RestImplementation restImpl)
        {
            Group testGroup = TemporaryGroupHelper.GetTestGroup();
            
            // Create group
            RestResponse creationResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Create(testGroup);

            // Delete test group
            const string jsonStart = "http://dev2.ndexbio.org/v2/group/";
            Guid createdGroupId = new Guid(creationResponse.json.Substring(jsonStart.Length));
            await TemporaryGroupHelper.DeleteTestGroup(createdGroupId, restImpl);
            
            Assert.NotNull(creationResponse);
            Assert.True(creationResponse.wasSuccess);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void DeleteGroupWithError(RestImplementation restImpl)
        {
            Guid createdGroupId = (await TemporaryGroupHelper.CreateTestGroup(restImpl)).externalId;

            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .Delete(Guid.Empty));

            await TemporaryGroupHelper.DeleteTestGroup(createdGroupId, restImpl);

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("User 9f762cdf-699b-11e9-831d-0660b7976219 " +
                         "doesn't have permission to delete group " + Guid.Empty, exception.Message);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void DeleteGroup(RestImplementation restImpl)
        {
            Guid createdGroupId = (await TemporaryGroupHelper.CreateTestGroup(restImpl)).externalId;

            RestResponse restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Delete(createdGroupId);
            
            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Get group and check for update
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .Get(createdGroupId));
            
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Group with UUID: " + createdGroupId + " doesn't exist.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void UpdateGroupWithError(RestImplementation restImpl)
        {
            Guid createdGroupId = (await TemporaryGroupHelper.CreateTestGroup(restImpl)).externalId;
            
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .Update(TemporaryGroupHelper.GetTestGroup(), Guid.Empty));

            await TemporaryGroupHelper.DeleteTestGroup(createdGroupId, restImpl);
            
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Only group administrators can update a group.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void UpdateGroup(RestImplementation restImpl)
        {
            Group createdGroup = await TemporaryGroupHelper.CreateTestGroup(restImpl);
            Guid createdGroupId = createdGroup.externalId;
            Group testGroup = TemporaryGroupHelper.GetTestGroupUpdated();
            testGroup.externalId = createdGroup.externalId;
            testGroup.creationTime = createdGroup.creationTime;
            testGroup.modificationTime = createdGroup.modificationTime;
            testGroup.isDeleted = createdGroup.isDeleted;

            RestResponse restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Update(testGroup, createdGroupId);
            
            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Get group and check for updates
            Group group = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Get(createdGroupId);
            
            await TemporaryGroupHelper.DeleteTestGroup(createdGroupId, restImpl);
            
            Assert.NotEmpty(group.groupName);
            Assert.Equal(testGroup.image, group.image);
            Assert.Equal(testGroup.description, group.description);
            Assert.Equal(testGroup.website, group.website);
            Assert.Equal(testGroup.properties, group.properties);
            Assert.Equal(testGroup.externalId, group.externalId);
            Assert.Equal(testGroup.isDeleted, group.isDeleted);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetMembersWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .GetGroupMembers(Guid.Empty, null, 0, 0));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Group with UUID: " + Guid.Empty + " doesn't exist.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void UpdateMemberWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .AddOrUpdateGroupMember(Guid.Empty, SharedIds.UserId2, Permissions.ADMIN));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Only group admin can update membership.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void RemoveMemberWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .RemoveGroupMember(Guid.Empty, SharedIds.UserId2));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Unable to delete group membership: user need to be an admin of this group or can only make himself leave this group.", 
                exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetAndUpdateMember(RestImplementation restImpl)
        {
            // Check amount of admins in group before test
            List<Membership> memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, Permissions.GROUPADMIN, 0, 10);
            Assert.Single(memberships);

            // Make user 2 to admin
            RestResponse restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .AddOrUpdateGroupMember(SharedIds.GroupId1, SharedIds.UserId2, Permissions.GROUPADMIN);

            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Check amount of admins in group after test
            memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, Permissions.GROUPADMIN, 0, 10);
            Assert.Equal(2, memberships.Count);
            
            // Make user 2 only to member again
            restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .AddOrUpdateGroupMember(SharedIds.GroupId1, SharedIds.UserId2, Permissions.MEMBER);

            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Check amount of admins in group after test
            memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, Permissions.GROUPADMIN, 0, 10);
            Assert.Single(memberships);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetRemoveAndAddMember(RestImplementation restImpl)
        {
            // Check amount of users in group before test
            List<Membership> memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, null, 0, 10);
            Assert.Equal(2, memberships.Count);

            // Remove user 2 before actual test
            RestResponse restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .RemoveGroupMember(SharedIds.GroupId1, SharedIds.UserId2);

            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Check amount of users in group after test
            memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, null, 0, 10);
            Assert.Single(memberships);
            
            // Add user 2 to group
            restResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .AddOrUpdateGroupMember(SharedIds.GroupId1, SharedIds.UserId2, Permissions.MEMBER);

            Assert.NotNull(restResponse);
            Assert.True(restResponse.wasSuccess);
            
            // Check amount of users in group after test
            memberships = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetGroupMembers(SharedIds.GroupId1, null, 0, 10);
            Assert.Equal(2, memberships.Count);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetPermissionsType1WithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .GetPermissionsOfGroup(Guid.Empty, SharedIds.NetworkId2));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Only a group member or admin can check group permission on a network", 
                exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetPermissionsType1(RestImplementation restImpl)
        {
            Dictionary<Guid, Permissions> permissions = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetPermissionsOfGroup(SharedIds.GroupId1, SharedIds.NetworkId2);

            Assert.NotNull(permissions);
            Assert.Single(permissions);
            Assert.Equal(Permissions.WRITE, permissions[SharedIds.NetworkId2]);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetPermissionsType2WithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Group()
                    .GetPermissionsOfGroupMembers(Guid.Empty, null, 0, 10));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Group with UUID: " + Guid.Empty + " doesn't exist.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetPermissionsType2(RestImplementation restImpl)
        {
            List<Membership> permissions = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .GetPermissionsOfGroupMembers(SharedIds.GroupId1, Permissions.WRITE, 0, 10);

            Assert.NotNull(permissions);
            Assert.Equal(2, permissions.Count);
            Assert.Equal(SharedIds.GroupId1, permissions[0].resourceUUID);
            Assert.Equal(SharedIds.GroupId1, permissions[1].resourceUUID);
        }
    }
}