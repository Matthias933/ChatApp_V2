using System;
using System.Numerics;

namespace ObserverPattern
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Erstell das Subject und 2 Observer
            Subject subject =new Subject();
            ObserverA observerA = new ObserverA();
            ObserverB observerB = new ObserverB(); 

            //Fügt die Observer zu dem Subject hinzu
            subject.Attach(observerA);
            subject.Attach(observerB);

            //Führt einen kleinen logic teil aus
            subject.Logic();
            subject.Logic();

            //Entfernt einen Observer wieder
            subject.Detach(observerB);

            subject.Logic();
        }
    }

    public interface IObserver
    {
        //Bekommt Updateds von dem Subject
        public void Update(ISubject subject);
    }

    public interface ISubject
    {
        //Einen Observer anhängen
        public void Attach(IObserver observer);

        //Entferent einen Observer wieder
        public void Detach(IObserver observer);

        //Informiert einen Observer
        public void Notify();
    }

    public class Subject : ISubject
    {

        public int State { get; set; } = -1;

        private List<IObserver> observers = new List<IObserver>();
        public void Attach(IObserver observer)
        {
            Console.WriteLine("Subject Atached an Observer");
            observers.Add(observer);    
        }

        public void Detach(IObserver observer)
        {
            Console.WriteLine("Subject Detached an Observer");
            observers.Remove(observer);
        }

        //Informiert alle observer die auf dem Subject sind
        public void Notify()
        {
            Console.WriteLine("Subject: Notifying Observers");

            foreach (IObserver observer in observers)
            {
                observer.Update(this);
            }
        }

        public void Logic()
        {
            Console.WriteLine("Subject is doing something...");
            State = new Random().Next(0, 10);

            Console.WriteLine($"Finished my state has changed to {State}");
            Notify();
        }

    }

    public class ObserverA : IObserver
    {
        public void Update(ISubject subject)
        {
            if((subject as Subject).State < 3)
            {
                Console.WriteLine("Observer A reacted to the event");
            }
        }
    }

    public class ObserverB : IObserver
    {
        public void Update(ISubject subject)
        {
            if((subject as Subject).State == 0 || (subject as Subject).State >= 2)
            {
                Console.WriteLine("Observer B reacted to the event");
            }
        }
    }
}