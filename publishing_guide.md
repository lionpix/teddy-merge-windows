# TeddyMerge - Microsoft Store Publishing Guide

### Prerequisites
Before you begin, ensure you have:
- A completed, tested application.
- A **Microsoft Developer Account** (costs ~$19 once for individuals, or ~$99 for companies). You can register at the [Microsoft Partner Center](https://partner.microsoft.com/en-us/dashboard/windows/overview).
- Visual Studio installed with the **Universal Windows Platform development** or **Windows App SDK** workloads.

---

### Step 1: Reserve Your App Name
1. Log in to the [Microsoft Partner Center](https://partner.microsoft.com/dashboard/windows/overview).
2. Go to **Apps and games** and click **New product** -> **MSIX or PWA app**.
3. Enter the name you want for your app (e.g., "TeddyMerge") and click **Check availability**.
4. If it's available, click **Reserve product name**.

> [!NOTE]
> Reserving the name holds it for you for 3 months, giving you time to finalize and submit the app.

---

### Step 2: Associate Your App with the Store (Visual Studio)
You need to link your local project to the name you just reserved in the Store.

1. Open your solution (`.sln`) in **Visual Studio**.
2. Right-click on your main project (the one containing `Package.appxmanifest`) in the **Solution Explorer**.
3. Select **Publish** -> **Associate App with the Store...**.
4. Follow the prompts to sign in to your Microsoft Developer account.
5. Select the app name you reserved in Step 1 and click **Next** then **Associate**.

*(This updates your `Package.appxmanifest` with the correct Package Identity, Publisher ID, and Publisher Display Name automatically).*

---

### Step 3: Create the App Packages for the Store
Now you need to generate the `.msixupload` or `.msixbundle` file that the Store requires.

1. Keep Visual Studio open.
2. Right-click your project again, go to **Publish** -> **Create App Packages...**.
3. Select **Microsoft Store under a new app name** (or the name you just associated if it asks).
4. On the **Select and Configure Packages** screen:
   - Make sure the output location is somewhere you can easily find.
   - For **Architecture**, select `x64` and `ARM64` (and `x86` if you wish to support 32-bit systems). 
   - Set the **Generate app bundle** dropdown to **Always**.
5. Click **Create**.
6. Visual Studio will build your project and run the Windows App Certification Kit (WACK) automatically to test for Store compliance. 
   - *Ensure the WACK tests pass. If they fail, review the errors, fix them in your code, and recreate the packages.*

---

### Step 4: Create a New Submission in Partner Center
Once your `.msixupload` file is ready, go back to the Partner Center.

1. Navigate to your app in the **Partner Center** and click **Start your submission**.
2. You will see several sections to fill out:
   - **Pricing and availability:** Set your price (Free or Paid), target markets, and release schedule.
   - **Properties:** Select the category and subcategory (e.g., Productivity).
   - **Age ratings:** Complete the questionnaire to generate an age rating for your app.
   - **Store listings:** This is what users see. You will need:
     - A thoughtful description.
     - Search terms (keywords).
     - Store logos (usually a 300x300 logo).
     - Screenshots of your app in action (at least one, but up to 10 are recommended).

---

### Step 5: Upload Your Packages
1. Still in the Partner Center submission flow, go to the **Packages** section.
2. Drag and drop the `.msixupload` (or `.msixbundle`) file you generated in Step 3 into the upload box.
3. Once uploaded and validated, click **Save**.

---

### Step 6: Submit for Certification
1. Review all the sections in your submission. Ensure none of them say "Incomplete".
2. At the top right, click **Submit to the Store**.

### What happens next?
- **Certification:** Microsoft will run automated and manual tests on your app. This can take anywhere from a few hours to a few days.
- **Publishing:** Once it passes certification, it will be published to the Microsoft Store based on the timeline you defined in the "Pricing and availability" section (usually within 24 hours). 

> [!TIP]
> Keep an eye on the email associated with your developer account. If your app fails certification, Microsoft will send an email detailing exactly what policy was violated and how to fix it.
