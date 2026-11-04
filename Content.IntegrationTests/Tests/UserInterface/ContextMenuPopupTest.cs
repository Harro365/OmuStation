// SPDX-License-Identifier: MIT

using Content.Client.ContextMenu.UI;
using Robust.Client.UserInterface;

namespace Content.IntegrationTests.Tests.UserInterface;

[TestFixture]
public sealed class ContextMenuPopupTest
{
    [Test]
    public async Task RemovingAnEntityWithAnOpenSubmenuDoesNotCrash()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Dirty = true });
        await pair.Client.WaitAssertion(() =>
        {
            var ui = pair.Client.ResolveDependency<IUserInterfaceManager>();
            var controller = ui.GetUIController<ContextMenuUIController>();
            var parent = new ContextMenuElement("Entity");
            var popup = new ContextMenuPopup(controller, parent);
            popup.MenuBody.AddChild(new ContextMenuElement("Action"));
            // EntityMenuUIController registers this callback on entity submenus.
            popup.OnPopupHide += popup.MenuBody.RemoveAllChildren;
            popup.Open();

            Assert.DoesNotThrow(() => parent.Dispose());
            Assert.That(popup.Disposed, Is.True);
            Assert.That(popup.Visible, Is.False);
            Assert.DoesNotThrow(() => parent.Dispose());
        });
        await pair.CleanReturnAsync();
    }
}
