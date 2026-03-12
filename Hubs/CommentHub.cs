using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.Hubs
{
    [Authorize]
    public class CommentHub : Hub
    {
        public async Task JoinInventoryGroup(int inventoryId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"inventory-{inventoryId}");
        }

        public async Task LeaveInventoryGroup(int inventoryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"inventory-{inventoryId}");
        }

        public async Task SendComment(int inventoryId, string content, string userName)
        {
            await Clients.Group($"inventory-{inventoryId}").SendAsync("ReceiveComment", new
            {
                content,
                userName,
                createdAt = DateTime.UtcNow
            });
        }
    }
}