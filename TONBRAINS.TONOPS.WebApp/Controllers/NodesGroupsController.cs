using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/groups")]
    public class NodesGroupsController : SessionController
    {
        private readonly INodeSvc _nodeService;
        public NodesGroupsController(INodeSvc nodeService)
        {
            _nodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }
        [Route("grid")]
        public async Task<IEnumerable<NodeGroupModel>> Groups(string name)
        {

            return (await ExecuteQuery<NodeGroupModel>(new Query("node_groups"))).Where(x => x.Name != name);
        }

            

        [Route("single")]
        public async Task<NodeGroupModel> Group(string name)
            => await ExecuteQueryFirst<NodeGroupModel>(new Query("node_groups").Where("name", name));


        [Route("nodegroupgrid")]
        public async Task<IEnumerable<NodeGroupModel>> NodeGtroupGrid()
        {

            var rs = await ExecuteQuery<NodeGroupModel>(new Query("v_node_groups"));

            return rs;
        }

        [Route("addoredit")]
        [HttpGet]
        public async Task<NodeGroup> AddOrEdit(string after, string before)
        {
            if (!string.IsNullOrEmpty(after))
            {
                var id = Guid.NewGuid();
                var afterModel = await Group(after);
                if (afterModel != null)
                {
                    return await ExecuteInsertOrUpdate(
                        new NodeGroup
                        {
                            Id = afterModel.Id,
                            Name = before
                        },
                        new Query("node_groups"),
                        insert: false
                    );
                }
            }
            return await ExecuteInsertOrUpdate(
                new NodeGroup
                {
                    Id = IdGenerator.Generate(),
                    Name = before
                },
                new Query("node_groups"),
                insert: true
            );
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id) => await _nodeService.DeleteNodeGroup(id);

    }
}
