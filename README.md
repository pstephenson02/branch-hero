# Branch Hero

Branch Hero is here to protect your GitHub repository default branches across an entire organization. It automatically applies the following GitHub branch protection rules for all new default branches across repositories in an organization:

* Require pull request before merging
* Require at least 1 approval before merging
* Dismiss stale pull request approvals when new commits are pushed

## How It Works

Branch Hero works by listening for two specific repository events:
* When the **first branch in any new repository is created**. By convention, the first branch in any new GitHub repository becomes the default branch (even if the organization is configured with a different default branch name).
* When the **default branch for any repository has been changed**.

**Caveat**: If a default branch change occurs, BH will first check if there already exists a branch protection rule for the new default branch. If one already exists, BH leaves this alone and does nothing.

### Prerequisites

* An Azure account. [Sign up for free](https://azure.microsoft.com/en-us/free/).
* A GitHub organization. [Sign up for free](https://github.com/join).
* A GitHub user with admin access to the organization.

## Installation

The simplest way to run Branch Hero is to follow these instructions to deploy an instance of the service into Azure.

1. Start by creating a fork of this repository. In the top-right corner of the page, click **Fork**.
![GitHub Fork](https://docs.github.com/assets/cb-6294/images/help/repository/fork_button.jpg)

2. Next, click the **Deploy to Azure** button to deploy the Azure infrastructure:
<br />
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpstephenson02%2Fbranch-hero%2Fmain%2Fazuredeploy.arm.json" target="_blank">
  <img src="https://aka.ms/deploytoazurebutton"/>
</a>
<br />
You'll be prompted first to login to Azure. Then, you should see the following page:
<img src="assets/azuredeploy.PNG" width="600" />
<br />
The default values will create a unique name for your instance, but you can of course change these to your liking.<br /><br />
Once your deployment is complete, use the Azure search bar and find your new Azure App Service. If you used the default settings above, your App Service name should look something like: <b>branch-hero20220322T220512Z</b>
<br />
On the App Service page, take note of the public URL. We'll need this in our next step.
<br />
<img src="assets/appservice.PNG" width="900" />

3. As your GitHub user with admin access to your organization, go to [your organization's settings page](https://docs.github.com/en/organizations/collaborating-with-groups-in-organizations/accessing-your-organizations-settings). Find the Webhooks section, and [create a new Webhook for your organization](https://docs.github.com/en/developers/webhooks-and-events/webhooks/creating-webhooks).
    * In the Payload URL field, use the URL to our new Azure App Service from the previous step, plus the route suffix: /api/webhooks. For example: **https://branch-hero20220321195020.azurewebsites.net/api/webhooks**
    * Change the Content type from **application/x-www-form-urlencoded** to **application/json**
    * For the Secret field, enter a random sequence of characters. It's best to use a completely random string with high entropy. This string will be used to secure your instance of Branch Hero's public facing API. **Keep track of your secret** - you'll need it soon to configure your Branch Hero app. [Read more about the Secret field here](https://docs.github.com/en/developers/webhooks-and-events/webhooks/securing-your-webhooks#setting-your-secret-token).
    * Under the question: **Which events would you like to trigger this webhook?** Select the option **Let me select individual events.** Branch only acts upon two event types:
        * **Branch or tag creation**
        * **Repositories**
<br />Select these two and you can remove any other predefined defaults. 
    * Scroll down and make sure the Active checkbox is checked, and click <b>Add webhook</b>.
<br />
<br />
<img src="assets/addwebhook.PNG" width="600" />

