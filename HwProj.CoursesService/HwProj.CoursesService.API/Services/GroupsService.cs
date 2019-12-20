﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HwProj.CoursesService.API.Models;
using HwProj.CoursesService.API.Models.DTO;
using HwProj.CoursesService.API.Repositories;
using HwProj.CoursesService.API.Repositories.Groups;
using Microsoft.EntityFrameworkCore;

namespace HwProj.CoursesService.API.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly IGroupsRepository _groupsRepository;
        private readonly IGroupMatesRepository _groupMatesRepository;
        private readonly ITaskModelsRepository _taskModelsRepository;
        private readonly IMapper _mapper;

        public GroupsService(IGroupsRepository groupsRepository,
            IGroupMatesRepository groupMatesRepository,
            ITaskModelsRepository taskModelsRepository,
            IMapper mapper)
       
        {
            _groupsRepository = groupsRepository;
            _groupMatesRepository = groupMatesRepository;
            _taskModelsRepository = taskModelsRepository;
            _mapper = mapper;
        }

        public async Task<Group[]> GetAllAsync(long courseId)
        {
            return await _groupsRepository.GetGroupsWithGroupMatesByCourse(courseId).ToArrayAsync().ConfigureAwait(false);
        }

        public async Task<Group> GetGroupAsync(long groupId)
        {
            return await _groupsRepository.GetGroupWithGroupMatesAsync(groupId).ConfigureAwait(false);
        }

        public async Task<long> AddGroupAsync(Group group)
        { 
            var groupId = await _groupsRepository.AddAsync(group).ConfigureAwait(false);

            return groupId;
        }

        public async Task AddGroupMateAsync(long groupId, string studentId)
        {
            var groupMate = new GroupMate
            {
                GroupId = groupId,
                StudentId = studentId,
            };

            await _groupMatesRepository.AddAsync(groupMate).ConfigureAwait(false);
        }

        public async Task DeleteGroupAsync(long groupId)
        {
            await _groupsRepository.DeleteAsync(groupId).ConfigureAwait(false);
        }

        public async Task UpdateAsync(long groupId, Group updated)
        {
            await _groupsRepository.UpdateAsync(groupId, c => new Group
            {
                Name = updated.Name,
            });
        }

        public async Task<bool> DeleteGroupMateAsync(long groupId, string studentId)
        {
            var group = await _groupsRepository.GetAsync(groupId).ConfigureAwait(false);
            if (group == null)
            {
                return false;
            }

            var getGroupMateTask =
                await _groupMatesRepository.FindAsync(cm => cm.GroupId == groupId && cm.StudentId == studentId).ConfigureAwait(false);

            if (getGroupMateTask == null)
            {
                return false;
            }


            await _groupMatesRepository.DeleteAsync(getGroupMateTask.Id);
            return true;
        }

        public async Task<UserGroupDescription[]> GetStudentGroupsAsync(long courseId, string studentId)
        {
            var studentGroupsIds = await _groupMatesRepository
                .FindAll(cm => cm.StudentId == studentId)
                .Select(cm => cm.GroupId)
                .ToArrayAsync()
                .ConfigureAwait(false);

            var getStudentGroupsTask = studentGroupsIds
                .Select(async id => await _groupsRepository.GetAsync(id).ConfigureAwait(false))
                .Where(cm => cm.Result.CourseId == courseId)
                .ToArray();
            var studentGroups = await Task.WhenAll(getStudentGroupsTask).ConfigureAwait(false);

            return studentGroups.Select(c => _mapper.Map<UserGroupDescription>(c)).ToArray();
        }

        public async Task<long[]> GetTasksIds(long groupId)
        {
            var getGroupTask = await _groupsRepository.GetAsync(groupId);
            return getGroupTask.Tasks.Select(cm => cm.TaskId).ToArray();
        }
    }
}