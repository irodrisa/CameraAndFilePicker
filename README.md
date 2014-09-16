CameraAndFilePicker
===================

Universal Windows app that combines CaptureElement and MediaCapture (for a simple camera preview) with the File Picker.

This Visual Studio project uses elements and classes from the Universal Windows app samples:
  - NavigationHelper
  - SuspensionManager
  - CaptureElement
  - MediaCapture
  - FileOpenPicker
  - FutureAccessList

The code shows how to properly manage navigation events as well as suspend/resume events.
The UI is 100% shared between the Windows and Windows Phone projects.

This project was created with help obtained from StackOverflow through the following questions:
  - Navigate to a different page from ContinueFileOpenPicker method (http://stackoverflow.com/questions/25817100/navigate-to-a-different-page-from-continuefileopenpicker-method)
  - Handling Resuming event for MediaCapture/CaptureElement when combined with the File Picker sample (http://stackoverflow.com/questions/25754997/handling-resuming-event-for-mediacapture-captureelement-when-combined-with-the)
  - SuspensionManager in Windows Universal Apps doesn't do anything on fast app switching (http://stackoverflow.com/questions/25319121/suspensionmanager-in-windows-universal-apps-doesnt-do-anything-on-fast-app-swit)
