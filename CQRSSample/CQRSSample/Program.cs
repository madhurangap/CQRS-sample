using System;
using System.Collections.Generic;

namespace CQRSSample
{

    //COMMAND = do/change

    public class Person
    {
        private int _age;

        private readonly EventBroker _broker;

        public Person(EventBroker broker)
        {
            _broker = broker;

            _broker.Commands += BrokerOnCommands;
            _broker.Queries += BrokerOnQueries;
        }

        private void BrokerOnQueries(object sender, Query q)
        {
            var ac = q as AgeQuery;

            if (ac != null && ac.Target == this)
            {
                ac.Result = _age;
            }
        }

        private void BrokerOnCommands(object sender, Command c)
        {
            var cac = c as ChangeAgeCmmand;

            if (cac != null && cac.Target == this)
            {
                _age = cac.Age;
            }
        }
    }


    public class EventBroker
    {
        //1. All events that happends
        public IList<Event> AllEvents = new List<Event>();

        //2.Commands
        public event EventHandler<Command> Commands;

        //3.Queries
        public event EventHandler<Query> Queries;

        public void Command(Command c)
        {
            Commands?.Invoke(this, c);
        }

        public T Query<T>(Query q)
        {
            Queries?.Invoke(this, q);
            return (T)q.Result;
        }
    }

    public class Query
    {
        public Object Result;
    }

    public class AgeQuery : Query
    {
        public Person Target;
    }

    public class Command : EventArgs
    {

    }

    public class ChangeAgeCmmand : Command
    {
        public Person Target;
        public int Age;

        public ChangeAgeCmmand(Person target, int age)
        {
            Target = target;
            Age = age;
        }
    }

    public class Event
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            var eb = new EventBroker();
            var p = new Person(eb);

            eb.Command(new ChangeAgeCmmand(p, 123));

            var age = eb.Query<int>(new AgeQuery() { Target = p });

            Console.WriteLine(age);

            Console.ReadKey();
        }
    }
}