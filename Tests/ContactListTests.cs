using Shared;
using Xunit;

namespace Tests
{
    public class ContactListTests
    {
        [Fact]
        public void TestAddContact()
        {
            var contactList = new RemoteContactList();
            bool result = contactList.AddContact("TestUser", true);

            Assert.True(result);
            Assert.Single(contactList.Contacts);
            Assert.Equal("TestUser", contactList.Contacts[0].Item1);
            Assert.True(contactList.Contacts[0].Item2);
        }

        [Fact]
        public void TestRemoveContact()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("UserToRemove", false);
            bool removed = contactList.RemoveContact("UserToRemove");

            Assert.True(removed);
            Assert.Empty(contactList.Contacts);
        }

        [Fact]
        public void TestUpdateContactStatus()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("User1", false);
            contactList.AddContact("User1", true); // Update to online

            Assert.Single(contactList.Contacts);
            Assert.True(contactList.Contacts[0].Item2);
        }

        [Fact]
        public void TestGetContact()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);

            var alice = contactList.GetContact("Alice");
            var bob = contactList.GetContact("Bob");
            var charlie = contactList.GetContact("Charlie");

            Assert.NotNull(alice);
            Assert.Equal("Alice", alice.Name);
            Assert.True(alice.IsOnline);

            Assert.NotNull(bob);
            Assert.Equal("Bob", bob.Name);
            Assert.False(bob.IsOnline);

            Assert.Null(charlie);
        }

        [Fact]
        public void TestRemoveNonExistent()
        {
            var contactList = new RemoteContactList();
            bool result = contactList.RemoveContact("NonExistentUser");

            Assert.False(result);
        }

        [Fact]
        public void TestEmptyUsername()
        {
            var contactList = new RemoteContactList();
            bool result1 = contactList.AddContact("", true);
            bool result2 = contactList.AddContact(null!, false);
            bool result3 = contactList.RemoveContact("");

            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
        }

        [Fact]
        public void TestContains()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("Alice", true);

            Assert.True(contactList.Contains("Alice"));
            Assert.False(contactList.Contains("Bob"));
        }

        [Fact]
        public void TestGetAllContacts()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);
            contactList.AddContact("Charlie", true);

            var allContacts = contactList.GetAllContacts();

            Assert.Equal(3, allContacts.Count);
            Assert.Contains(allContacts, c => c.Name == "Alice" && c.IsOnline);
            Assert.Contains(allContacts, c => c.Name == "Bob" && !c.IsOnline);
            Assert.Contains(allContacts, c => c.Name == "Charlie" && c.IsOnline);
        }

        [Fact]
        public void TestContactsProperty()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);

            var contacts = contactList.Contacts;

            Assert.Equal(2, contacts.Count);
            Assert.Contains(contacts, c => c.Item1 == "Alice" && c.Item2 == true);
            Assert.Contains(contacts, c => c.Item1 == "Bob" && c.Item2 == false);
        }

        [Fact]
        public void TestUpdateContactStatusMethod()
        {
            var contactList = new RemoteContactList();
            contactList.AddContact("Alice", false);

            contactList.UpdateContactStatus("Alice", true);

            var alice = contactList.GetContact("Alice");
            Assert.NotNull(alice);
            Assert.True(alice.IsOnline);
        }

        [Fact]
        public void TestCount()
        {
            var contactList = new RemoteContactList();

            Assert.Equal(0, contactList.Count);

            contactList.AddContact("Alice", true);
            Assert.Equal(1, contactList.Count);

            contactList.AddContact("Bob", false);
            Assert.Equal(2, contactList.Count);

            contactList.RemoveContact("Alice");
            Assert.Equal(1, contactList.Count);
        }

        // Integration tests from ContactListDemo
        [Fact]
        public void TestCompleteWorkflow_AddUpdateRemove()
        {
            var contactList = new RemoteContactList();

            // Initial state - empty
            Assert.Empty(contactList.Contacts);

            // Add contacts with different statuses
            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);
            contactList.AddContact("Charlie", true);

            Assert.Equal(3, contactList.Count);
            Assert.True(contactList.Contains("Alice"));
            Assert.True(contactList.Contains("Bob"));
            Assert.True(contactList.Contains("Charlie"));

            // Update Bob's status to online
            contactList.AddContact("Bob", true);
            var bob = contactList.GetContact("Bob");
            Assert.NotNull(bob);
            Assert.True(bob.IsOnline);

            // Remove Charlie
            bool removed = contactList.RemoveContact("Charlie");
            Assert.True(removed);
            Assert.False(contactList.Contains("Charlie"));
            Assert.Equal(2, contactList.Count);
        }

        [Fact]
        public void TestCompleteWorkflow_InvalidOperations()
        {
            var contactList = new RemoteContactList();

            // Add some valid contacts first
            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);

            // Try invalid add operations
            bool emptyAdd = contactList.AddContact("", true);
            bool nullAdd = contactList.AddContact(null!, false);

            Assert.False(emptyAdd);
            Assert.False(nullAdd);
            Assert.Equal(2, contactList.Count); // Should still be 2

            // Try invalid remove operations
            bool emptyRemove = contactList.RemoveContact("");
            bool nullRemove = contactList.RemoveContact(null!);
            bool nonExistentRemove = contactList.RemoveContact("David");

            Assert.False(emptyRemove);
            Assert.False(nullRemove);
            Assert.False(nonExistentRemove);
            Assert.Equal(2, contactList.Count); // Should still be 2
        }

        [Fact]
        public void TestCompleteWorkflow_ContactsAndTuples()
        {
            var contactList = new RemoteContactList();

            contactList.AddContact("Alice", true);
            contactList.AddContact("Bob", false);
            contactList.AddContact("Charlie", true);

            // Test GetAllContacts (returns Contact objects)
            var allContacts = contactList.GetAllContacts();
            Assert.Equal(3, allContacts.Count);
            Assert.All(allContacts, c => Assert.NotNull(c.Name));

            // Test Contacts property (returns tuples)
            var contactTuples = contactList.Contacts;
            Assert.Equal(3, contactTuples.Count);

            var aliceTuple = contactTuples.FirstOrDefault(c => c.Item1 == "Alice");
            Assert.Equal("Alice", aliceTuple.Item1);
            Assert.True(aliceTuple.Item2);

            var bobTuple = contactTuples.FirstOrDefault(c => c.Item1 == "Bob");
            Assert.Equal("Bob", bobTuple.Item1);
            Assert.False(bobTuple.Item2);
        }

        [Fact]
        public void TestCompleteWorkflow_MultipleUpdates()
        {
            var contactList = new RemoteContactList();

            // Add Alice as offline
            contactList.AddContact("Alice", false);
            var alice1 = contactList.GetContact("Alice");
            Assert.NotNull(alice1);
            Assert.False(alice1.IsOnline);

            // Update Alice to online
            contactList.AddContact("Alice", true);
            var alice2 = contactList.GetContact("Alice");
            Assert.NotNull(alice2);
            Assert.True(alice2.IsOnline);

            // Update Alice back to offline using UpdateContactStatus
            contactList.UpdateContactStatus("Alice", false);
            var alice3 = contactList.GetContact("Alice");
            Assert.NotNull(alice3);
            Assert.False(alice3.IsOnline);

            // Should still be only 1 contact
            Assert.Single(contactList.Contacts);
        }

        [Fact]
        public void TestCompleteWorkflow_RemoveAndReAdd()
        {
            var contactList = new RemoteContactList();

            // Add Alice
            contactList.AddContact("Alice", true);
            Assert.Single(contactList.Contacts);

            // Remove Alice
            bool removed = contactList.RemoveContact("Alice");
            Assert.True(removed);
            Assert.Empty(contactList.Contacts);

            // Re-add Alice with different status
            contactList.AddContact("Alice", false);
            Assert.Single(contactList.Contacts);

            var alice = contactList.GetContact("Alice");
            Assert.NotNull(alice);
            Assert.False(alice.IsOnline);
        }
    }
}
