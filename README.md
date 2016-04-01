# StripeEntities
A library for integrating Stripe.Net (https://github.com/jaymedavis/stripe.net) into your ASP.Net MVC web app via Entity Framework

Installation
------------
To use StripeEntities, either download the code using git, build the "StripeEntities" project, and include an assembly reference in your project to the "StripeEntities.dll" assembly.

OR

Install the package from NuGet (https://www.nuget.org/packages/StripeEntities/) using the following command in the package manager console in Visual Studio:

```
Install-Package StripeEntities
```

Installation
------------
The library simplifies integration of Stripe (via Stripe.Net) provides a set of Entity Framework-ready classes for storing subscription and subscription plan data, plus a helper class that uses these entities to operate against the Stripe.Net classes. This is intended to provide a convenient base for integrating Stripe with ASP.Net MVC SaaS web applications, relieving you of the burden of defining your own classes that shadow the necessary parts of the Stripe data in your system.

After you setup a Stripe account and follow the instructions for setting up Stripe.Net in your project, then you can setup the entities by doing the following:

1. Implement the ICustomerEntity interface - This provide Stripe with the necessary unique identication data for customers, off of which it hands the card information. This is normally your application's custom sub-class of the IdentityUser.
2. Extend the EntityPlanBase abstract class - This provides a description of your plans. Extending it allows you to add properties such as feature flags and maximum resource counts 
3. Extend the SubscriptionEntityBase abstract class - This provides linking the subscription to your relevant subscription unit, such as a user or a team in the system. 

Generally, you'll then make sure the classses implementing/extending ICustomerEntity, PlanEntityBase, and SubscriptionEntityBase are part of your DbContext sub-class. Then you are ready to use the StripeEntities.StripeManager class to interact with the Stripe API via Stripe.Net. Be sure to:

1. Create one or more plans by creating and saving some instances of SubscriptionPlan AND calling StripeManager.CreatePlan
2. Create one or more customers by creating and saving some instances of your IStripeUser implementer AND calling StripeManager.CreateOrUpdateCustomer
3. Create one or more subscriptions by creating and saving some instances of your SubscriptionEntityBase sub-class AND calling StripeManager.Subscribe

There are also methods for other operations, such as deleting subscription plan, unsubscribing from plans, and pulling default payment credentials for a user (the card that is charged for their subscription).
