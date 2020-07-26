using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class RoomTests
    {
        private HotelsContext _context;
        private RoomController _RoomController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _RoomController = new RoomController(_context);
        }

        [TestMethod]
        [Description("Get all rooms.")]
        public void GetRoomsTest()
        {
            Assert.IsNotNull(_context.Room, "Null. There is no rooms in DB.");
            Assert.IsNotNull(_RoomController.GetRooms().Value, "Null. API doesn't return any room.");
            Assert.IsTrue(_context.Room.Count() == _RoomController.GetRooms().Value.Count(), "Number of rooms from API and DB is not equal.");
        }

        [TestMethod]
        [Description("Get one room.")]
        public void GetRoomTest()
        {
            var id = Guid.Parse("222B3F14-A1D0-4B27-82D9-9208CE4FB917");
            Room room = _context.Room.Find(id);
            Room rezultat = _RoomController.GetRoom(id).Value;
            Assert.AreEqual(room, rezultat, "Room ID is not what is expected.");
        }

        [TestMethod]
        [Description("Get unexisting room.")]
        public void GetUnexistingRoomTest()
        {
            var id = Guid.Parse("B34DE1EA-1111-4E4A-9790-DA176B35B5EF");
            Assert.IsNull(_context.Room.Find(id), "Unexisting room is found on DB.");
            Assert.IsNull(_RoomController.GetRoom(id).Value, "Unexisting room is found by API.");
        }

        [TestMethod]
        [Description("Insert new room.")]
        public void PostRoomTest()
        {
            Room room = new Room();
            room.RoomNumber = 88;
            room.PricePerNight = 2880m;

            int roomBefore = _context.Room.Count();
            _RoomController.PostRoom(room);
            int roomAfter = _context.Room.Count();
            Assert.IsTrue(roomBefore < roomAfter, "Number of rooms before and after is equal.");
        }

        [TestMethod]
        [Description("Insert existing room.")]
        public void PostExistingRoomTest()
        {
            var id = Guid.Parse("222B3F14-A1D0-4B27-82D9-9208CE4FB917");
            Room room = _context.Room.Find(id);
            Assert.IsNotNull(room, "Null. The room is not in DB.");
            Room rezultat = _RoomController.PostRoom(room).Value;
            Assert.IsNull(rezultat, "Not null. Double room is inserted in DB.");
        }

        [TestMethod]
        [Description("Update room.")]
        public void PutRoomTest()
        {
            var id = Guid.Parse("2A04CDDC-5041-4D64-B802-56907BFE13C1");
            Room room = new Room();

            room.Id = id;
            room.RoomNumber = 55;
            room.PricePerNight = 1005m;

            _RoomController.PutRoom(room);
            Room roomAfter = _context.Room.Find(id);
            Assert.AreEqual(room.RoomNumber, roomAfter.RoomNumber, "Room.RoomNumber != RoomAfter.RoomNumber.");
            Assert.AreEqual(room.PricePerNight, roomAfter.PricePerNight, "Price per night is not the same.");
        }

        [TestMethod]
        [Description("Update unexisting room.")]
        public void PutUnexistingRoomTest()
        {
            var id = Guid.Parse("B34DE1EA-1111-4E4A-9790-DA176B35B5EF");
            Room room = new Room();

            room.Id = id;
            room.RoomNumber = 55;
            room.PricePerNight = 1005m;

            _RoomController.PutRoom(room);
            Assert.IsNull(_context.Room.Find(id), "Not null. Unexisting room is in DB.");
        }

        [TestMethod]
        [Description("Delete room.")]
        public void DeleteRoomTest()
        {
            var id = Guid.Parse("10B9B4E2-085D-4F78-8A07-97B47F44179E");

            Room room = _context.Room.Find(id);
            Assert.IsNotNull(room, "Null. Room is still in DB.");

            _RoomController.DeleteRoom(id);
            Assert.IsNull(_context.Room.Find(id), "Not null. Room was not deleted in DB.");

            _context.Add(room);
            _context.SaveChanges();
            Assert.IsNotNull(room, "Null. Room is not in DB, after insert.");
        }

        [TestMethod]
        [Description("Delete unexisting room.")]
        public void DeleteUnexistingRoomTest()
        {
            var id = Guid.Parse("222B3F14-1111-4B27-82D9-9208CE4FB917");
            Assert.IsNull(_RoomController.DeleteRoom(id).Value, "Not null. Room was not deleted from DB.");
        }
    }
}
