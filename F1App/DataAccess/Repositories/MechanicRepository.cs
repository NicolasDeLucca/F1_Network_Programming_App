using Domain;
using Exceptions;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public class MechanicRepository : IRepository<Mechanic>
    {
        private const int _adminId = 0;
        private List<Mechanic> _mechanics;
        private object _mechanicsLocker;
        private static object _instanceLocker = new object();
        private static MechanicRepository _instance;

        private MechanicRepository()
        {
            _mechanics = new List<Mechanic>();
            _mechanicsLocker = new object();
        }

        public static MechanicRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                    _instance = new MechanicRepository();

                return _instance;
            }
        }

        public Mechanic Get(int id)
        {
            lock (_mechanicsLocker)
            {
                if (id < 0 || id >= _mechanics.Count || _mechanics[id] == null)
                    throw new ResourceNotFoundException("Specified mechanic not found");
                
                return _mechanics[id]; 
            }
        }

        public Mechanic GetAdmin()
        {
            return Get(_adminId);
        }

        public List<Mechanic> GetAll()
        {
            lock (_mechanicsLocker)
            {
                return new List<Mechanic>(_mechanics);
            }
        }

        public int Store(Mechanic mechanic)
        {
            if (MechanicAlreadyExists(mechanic))
                throw new InvalidRequestDataException("Mechanic already exists");

            lock (_mechanicsLocker)
            {
                Mechanic newMechanic = new Mechanic()
                {
                    Id = _mechanics.Count,
                    Name = mechanic.Name,
                    Password = mechanic.Password
                };
                _mechanics.Add(newMechanic);

                return newMechanic.Id;
            }
        }

        public void Delete(int id)
        {
            Mechanic mechToDelete = Get(id);
            lock (_mechanicsLocker)
            {
                _mechanics.Remove(mechToDelete);
            }
        }

        private bool MechanicAlreadyExists(Mechanic mechanic)
        {
            lock (_mechanicsLocker)
            {
                return _mechanics.Find(m => m.Name == mechanic.Name) != null;
            }
        }

        //Mechanic Messages

        public List<Message> GetAllMessages(int id)
        {
            Mechanic mech = Get(id);
            lock (_mechanicsLocker)
            {
                return mech.Messages;
            }
        }

        public int StoreNewMessage(int receiverId, Message message)
        {
            Mechanic mechReceiver = Get(receiverId);
            lock (_mechanicsLocker)
            {
                return mechReceiver.AddMessage(message);
            }
        }

        public void ReadStoredMessage(int id, int messageId)
        {
            Mechanic mech = Get(id);
            lock (_mechanicsLocker)
            {
                mech.ReadMessage(messageId);
            }
        }
    }
}
