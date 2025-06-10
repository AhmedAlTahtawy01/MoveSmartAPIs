using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Hubs
{
    public class NotificationHub : Hub
    {
        // Called by clients when they connect and join a role-based group
        public async Task JoinRoleGroup(string roleName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roleName);
        }

        // Called by clients to leave the group (optional)
        public async Task LeaveRoleGroup(string roleName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roleName);
        }
    }
}
