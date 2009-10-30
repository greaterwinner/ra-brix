
********************************************************************************************


                Ra-Brix a Modular based Web Application Framework


********************************************************************************************

Ra-Brix is entirely the copyright of Thomas Hansen - thomas@ra-ajax.org and is licensed 
purely under GPL version 3 - unless permission to use under other terms are explicitly given.

Ra-Brix consists of several projects which each helps out with one problem in regards to mainly
web application development. The basic premise of Ra-Brix is that in order to be able to meet
dadlines, build scalable software with many developers on the same project one must take a
couple of measures. One is being able to completely abstract away the database. So Ra-Brix has
a modular database abstraction where providers can be built for every database in existance.
Another premise is that applications must be built in a modular fashion with extremely strong
separations between the different modules. Hence Ra-Brix have strong support for dynamically
loading modules completely unrelated of each other. Another is that these modules, even though
they are separated into isolated entities still need to communicate with each other. Hence
there is a very strong Event Dispatch system in Ra-Brix which makes it possible for different 
modules to communicate with each other. Another key point in building scalable system with 
high degree of flexibility is that one is able to reuse modules. This is being easily achieved
by a combination of that modules does not need to know about eachother and are completely
isolated in regards to whatever domain problem they solve. This means that whenever you start
a new project you can - if you have built your project correctly - reuse a lot of the modules
and the well known types into your next projects.

Another key point is that it is imperative to reduce the need for communication - ref; 
Mythical Man Month - Ra-Brix achieves that by making it possible to only focus on one problem
of the application and be able to do massive changes in this one area without affecting other
people's code or modules in any ways.

This makes it far faster to build and deliver software using Ra-Brix then what can be achieved 
with most other frameworks.