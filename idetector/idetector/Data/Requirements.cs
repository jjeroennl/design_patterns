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
            //DECORATOR-HAS-CHILDREN
            //DECORATOR-HAS-BASE-PROPERTY
            //DECORATOR-CONSTRUCTOR-SETS-COMPONENT
            //DECORATOR-CONCRETE-CALLS-BASE
            List<PatternRequirement> decoratorreqs = new List<PatternRequirement>();
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-HAS-CHILDREN", 1, "The base interface/abstract did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-CHILDREN", 1, "The abstract decorator did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-BASE-PROPERTY", 1, "The abstract decorator did not contain a field of the parent type"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", 1, "The abstract decorator' constructor does not set component field"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONCRETE-CALLS-BASE", 1, "The concrete decorators do not call the abstract decorator's constructor"));
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
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", 1, "There is no abstract class present that can function as a factory."));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-PRODUCT-INTERFACE", 1, "There is no interface present that can function as a product interface."));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", 1, "There is no abstract method found with the return type of a product interface."));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-PRODUCT-INTERFACE", 1, "There is no class that inherits an abstract (factory) class."));
            factoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT", 1, "There is no concrete factory that return a concrete product."));
            factoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS", 1, "Concrete factories should only have one method."));
            factoryreqs.Add(new PatternRequirement("FACTORY-ONE-PRODUCT-INTERFACE", 1, "Concrete factories should only have one method."));
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
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-INTERFACE-ABSTRACT", 4, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-HAS-CONTEXT", 3, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", 2, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-LOGIC", 1, ""));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONCRETE-CLASS", 2, ""));
            PatternRequirements.Add("STATE", statereqs);
            
            //FACADE
            /*ID's:
             * FACADE-IS-FACADE
             */
            List<PatternRequirement> facadereqs = new List<PatternRequirement>();
            facadereqs.Add(new PatternRequirement("FACADE-IS-FACADE", 4, ""));
            PatternRequirements.Add("FACADE", facadereqs);

            var strategyreqs = new List<PatternRequirement>(statereqs);
            strategyreqs.Add(new PatternRequirement("STRATEGY-CONCRETE-CLASS-RELATIONS", 2, ""));
            PatternRequirements.Add("STRATEGY", strategyreqs);



        }
        public Dictionary<string, List<PatternRequirement>> GetRequirements()
        {
            return PatternRequirements;
        }

    }
}
