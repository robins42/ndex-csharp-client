using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// Access to the REST API of any NDEx Server
    /// </summary>
    public class NDEx
    {
        private readonly Admin _admin;
        private readonly Batch _batch;
        private readonly Group _group;
        private readonly Network _network;
        private readonly NetworkSet _networkSet;
        private readonly Search _search;
        private readonly Task _task;
        private readonly User _user;

        /// <summary>
        /// Create a new NDEx connection
        /// </summary>
        /// <param name="network">Defines URL, proxy and authentication on NDEx</param>
        public NDEx(NetworkConfiguration network)
        {
            _admin = new Admin(network);
            _batch = new Batch(network);
            _group = new Group(network);
            _network = new Network(network);
            _networkSet = new NetworkSet(network);
            _search = new Search(network);
            _task = new Task(network);
            _user = new User(network);
        }

        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/admin</para>
        /// <para>The /admin endpoint allows users to access server information.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Admin Admin()
        {
            return _admin;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/batch</para>
        /// <para>The /batch endpoints allow users to do some tasks in a bigger batch job.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Batch Batch()
        {
            return _batch;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/group</para>
        /// <para>The /group endpoints allow users create and manage groups.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Group Group()
        {
            return _group;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/network</para>
        /// <para>The /network endpoints allow users create and manage single networks.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Network Network()
        {
            return _network;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/networkset</para>
        /// <para>The /networkset endpoints allow users create and manage networksets.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public NetworkSet NetworkSet()
        {
            return _networkSet;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/search</para>
        /// <para>The /search endpoints allow users search networks, groups or users.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Search Search()
        {
            return _search;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/task</para>
        /// <para>The /task endpoints allow users to manage their tasks or download exported networks.</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public Task Task()
        {
            return _task;
        }
        
        /// <summary>
        /// <para>Class to access all REST endpoints at baseUrl/user</para>
        /// <para>The /user endpoints allow users to see their permissions or saved networks</para>
        /// <para>Calling this method does not cost performance.</para>
        /// </summary>
        public User User()
        {
            return _user;
        }
    }
}