using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dvinun.DesignPatterns.Structural
{
    // In composite design pattern, we treat all parent, child equally.
    // In this demo, we use FamilyHead (leaf), Spouse (leaf), Family (composite), Kid (leaf) and all of them 
    // implement the IPerson class which is a component.
    class Composite
    {
        public static void PlayDemo()
        {
            FamilyHead Adam = new FamilyHead("Adam");
            Spouse Eve = new Spouse("Eve");
            Family AdamsFamily = new Family(Adam, Eve);

            FamilyHead John = new FamilyHead("John");
            Spouse Grace = new Spouse("Grace");
            Family JohnFamily = new Family(John, Grace);

            FamilyHead Stacy = new FamilyHead("Stacy");
            Spouse Peter = new Spouse("Peter");
            Family StacyFamily = new Family(Stacy, Peter);
            JohnFamily.AddKid(StacyFamily);
            JohnFamily.AddKid(new Kid("Bob"));

            AdamsFamily.AddKid(JohnFamily);

            Kid Lily = new Kid("Lily");
            AdamsFamily.AddKid(Lily);

            Kid Murphy = new Kid("Murphy");
            AdamsFamily.AddKid(Murphy);

            AdamsFamily.Display(1);
        }

        // component
        public interface IPerson
        {
            void AddKid(IPerson person);
            void SetSpouse(IPerson person);
            void SetName(IPerson person);

            List<IPerson> GetKids();
            IPerson GetSpouse();
            string GetName();

            void RemoveKid(IPerson person);

            void Display(int depth);
        }

        // leaf
        public class Kid : IPerson
        {
            string name;

            public Kid(string name)
            {
                this.name = name;
            }

            public void AddKid(IPerson person)
            {
                // not applicable
            }

            public void SetSpouse(IPerson person)
            {
                // not applicable
            }

            public void RemoveKid(IPerson person)
            {
                // not applicable
            }

            public List<IPerson> GetKids()
            {
                // not applicable
                return null;
            }

            public string GetName()
            {
                return name;
            }

            public IPerson GetSpouse()
            {
                // not applicable
                return null;
            }

            public void Display(int depth)
            {
                var depthDash = string.Concat(Enumerable.Repeat("-", depth));
                Console.WriteLine(depthDash + "Kid: " + name);
            }

            public void SetName(IPerson person)
            {
                // not applicable
            }
        }

        // leaf
        public class Spouse : IPerson
        {
            string name;

            public Spouse(string name)
            {
                this.name = name;
            }

            public void SetSpouse(IPerson person)
            {
                // not applicable
            }

            public void AddKid(IPerson person)
            {
                // not applicable
            }

            public void RemoveKid(IPerson person)
            {
                // not applicable
            }

            public List<IPerson> GetKids()
            {
                // not applicable
                return null;
            }

            public string GetName()
            {
                return name;
            }

            public IPerson GetSpouse()
            {
                // not applicable
                return null;
            }

            public void Display(int depth)
            {
                var depthDash = string.Concat(Enumerable.Repeat("-", depth));
                Console.WriteLine(depthDash + "Spouse: " + name);
            }

            public void SetName(IPerson person)
            {
                // not applicable
            }
        }

        // leaf
        public class FamilyHead : IPerson
        {
            string name;

            public FamilyHead(string name)
            {
                this.name = name;
            }

            public void SetSpouse(IPerson person)
            {
                // not applicable
            }

            public void AddKid(IPerson person)
            {
                // not applicable
            }

            public void RemoveKid(IPerson person)
            {
                // not applicable
            }

            public List<IPerson> GetKids()
            {
                // not applicable
                return null;
            }

            public string GetName()
            {
                return name;
            }

            public IPerson GetSpouse()
            {
                // not applicable
                return null;
            }

            public void Display(int depth)
            {
                var depthDash = string.Concat(Enumerable.Repeat("-", depth));
                Console.WriteLine(depthDash + "Family Head: " + name);
            }

            public void SetName(IPerson person)
            {
                // not applicable
            }
        }

        // composite
        public class Family : IPerson
        {
            IPerson familyHead;
            IPerson spouse;
            List<IPerson> children;

            public Family(IPerson familyHead, IPerson spouse)
            {
                children = new List<IPerson>();
                this.familyHead = familyHead;
                this.spouse = spouse;
            }

            public void AddKid(IPerson person)
            {
                children.Add(person);
            }

            public void SetSpouse(IPerson person)
            {
                this.spouse = person;
                // not applicable
            }

            public void RemoveKid(IPerson person)
            {
                // not implemented
            }

            public List<IPerson> GetKids()
            {
                // not applicable
                return children;
            }

            public string GetName()
            {
                // not applicable
                return null;
            }

            public IPerson GetSpouse()
            {
                return spouse;
            }

            public void Display(int depth)
            {
                var depthDash = string.Concat(Enumerable.Repeat("-", depth));
                Console.WriteLine(depthDash + "COUPLE");

                familyHead.Display(depth);
                spouse.Display(depth);

                if (children.Count > 0)
                {
                    var kidsdepthDash = string.Concat(Enumerable.Repeat("-", depth));
                    Console.WriteLine(kidsdepthDash + "KIDS");
                    foreach (IPerson person in children)
                    {
                        person.Display(depth + 2);
                    }
                }
            }

            public void SetName(IPerson person)
            {
                familyHead = person;
            }
        }
    }
}
