using idetector.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Data
{
    public class Requirements
    {
        Dictionary<string, List<PatternRequirement>> PatternRequirements = new Dictionary<string, List<PatternRequirement>>();

        public Requirements()
        {

            //SINGLETON
            /*ID's:
             * SINGLETON-PRIVATE-CONSTRUCTOR
             * SINGLETON-STATIC-SELF
             * SINGLETON-GET-INSTANCE
             * SINGLETON-CREATE-SELF
             */
            List<PatternRequirement> singletonreqs = new List<PatternRequirement>();
            singletonreqs.Add(new PatternRequirement("SINGLETON-PRIVATE-CONSTRUCTOR", 1, "Class does not contain a private constructor or the constructor is not private"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-STATIC-SELF", 1, "Class does not have a static field of itself or the field is not static"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-GET-INSTANCE", 1, "Class does not contain a method that returns the instance or returns more then one instance"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-CREATE-SELF", 1, "Class either does not create itself or creates itself multiple times"));
            PatternRequirements.Add("SINGLETON", singletonreqs);


            //DECORATOR
            //ID's:
            //DECORATOR-BASE-HAS-CHILDREN
            //DECORATOR-BASE-CHILDREN-TYPES
            //DECORATOR-HAS-CHILDREN
            //DECORATOR-HAS-BASE-PROPERTY
            //DECORATOR-CONSTRUCTOR-SETS-COMPONENT
            //DECORATOR-CONCRETE-CALLS-BASE
            List<PatternRequirement> decoratorreqs = new List<PatternRequirement>();
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-HAS-CHILDREN", 1, "The base decorator class did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-CHILDREN-TYPES", 10, "The base decorator did not contain a decorator child, or multiple decorator childs"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-CHILDREN", 1, "The decorator class did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-BASE-PROPERTY", 1, "The decorator class did not contain a field of the parent type"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", 1, "The decorator class' constructor does not set component field"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONCRETE-CALLS-BASE", 1, "The concrete decorators call the base decorator's constructor"));
            PatternRequirements.Add("DECORATOR", decoratorreqs);

            //ABSTRACT FACTORY METHOD
            /*ID's:
             *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
             *FACTORY-CONTAINS-PRODUCT-INTERFACE
             *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
             *FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS
             *FACTORY-INHERITING-PRODUCT-INTERFACE
             *FACTORY-RETURNS-PRODUCT         
             *FACTORY-MULTIPLE-METHODS
             */
            List<PatternRequirement> factoryreqs = new List<PatternRequirement>();
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-PRODUCT-INTERFACE", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-PRODUCT-INTERFACE", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS", 1, ""));
            PatternRequirements.Add("FACTORY", factoryreqs);

            //STATESTRATEGY
            /*ID's:
             * STATE-STRATEGY-HAS-CONTEXT
             * STATE-STRATEGY-CONTEXT-HAS-STRATEGY
             * STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY
             * STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR
             * STATE-STRATEGY-CONTEXT-STRATEGY-SETTER
             * STATE-STRATEGY-CONTEXT-LOGIC
             * STATE-STRATEGY-INTERFACE-ABSTRACT
             * STATE-STRATEGY-CONCRETE-CLASS
             * STATE-CONCRETE-CLASS-RELATIONS
             */
            List<PatternRequirement> statereqs = new List<PatternRequirement>();
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-HAS-CONTEXT", 3, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", 2, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-LOGIC", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-INTERFACE-ABSTRACT", 2, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONCRETE-CLASS", 2, ""));
            PatternRequirements.Add("STATE", statereqs);


            var strategyreqs = new List<PatternRequirement>(statereqs);
            strategyreqs.Add(new PatternRequirement("STRATEGY-CONCRETE-CLASS-RELATIONS", 2, ""));
            PatternRequirements.Add("STRATEGY", strategyreqs);

            //Command
            /*ID's:
             * COMMAND-HAS-INTERFACE
             * COMMAND-HAS-COMMAND-CLASS
             * COMMAND-HAS-PUBLIC-CONSTRUCTOR
             * COMMAND-HAS-RECEIVER-CLASS
             * COMMAND-USES-RECEIVER
             * COMMAND-HAS-INVOKER-CLASS
             */
            List<PatternRequirement> commandreqs = new List<PatternRequirement>();
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INTERFACE", 2, "The command pattern does not contain either an interface or an abstract class."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-COMMAND-CLASS", 3, "There are no commands that implement the abstract class or interface."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-PUBLIC-CONSTRUCTOR", 1, "One or all of the commands do not have a public constructor."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-RECEIVER-CLASS", 2, "There are no receiver classes implemented"));
            commandreqs.Add(new PatternRequirement("COMMAND-USES-RECEIVER",2, "None of the commands are controlled by a receiver"));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INVOKER-CLASS", 3, "The code does not contain an Invoker class"));
            PatternRequirements.Add("COMMAND", commandreqs);
        }
        public Dictionary<string, List<PatternRequirement>> GetRequirements()
        {
            return PatternRequirements;
        }

    }
}
