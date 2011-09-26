//
//  RenderTestView.m
//  fourthree-two
//
//  Created by Karl Schulze on 11-09-12.
//  Copyright 2011 TLA Digital Projects. All rights reserved.
//

#import "RenderTestView.h"

@implementation RenderTestView
@synthesize webView;

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    self.title = @"Output";
}

- (void)viewDidUnload
{
    [webView release];
    webView = nil;
    [self setWebView:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (IBAction)outputTestOneAction:(id)sender 
{
    NSBundle *bundle = [NSBundle mainBundle];
    NSString *filePath = [bundle pathForResource:@"LoremIpsum" ofType:@"txt"];
//    NSData *data = [NSData dataWithContentsOfFile:filePath];
    
    NSError *error;
    NSString *fileText = [NSString stringWithContentsOfFile:filePath encoding:NSUTF8StringEncoding error:&error];

    UIPrintInfo *printInfo = [UIPrintInfo printInfo];
    
    printInfo.jobName = @"Test Output One";
    if([UIPrintInteractionController isPrintingAvailable])
    {
        Class printControllerClass = NSClassFromString(@"UIPrintInteractionController");
        if (printControllerClass) 
        {
            printController = [printControllerClass sharedPrintController];
            
            UISimpleTextPrintFormatter *printFormatter = [[UISimpleTextPrintFormatter alloc] initWithText:fileText];
            
            void (^completionHandler)(UIPrintInteractionController *, BOOL, NSError *) =
            ^(UIPrintInteractionController *pic, BOOL completed, NSError *error) 
            {
                if (!completed && error) DLog(@"Print error: %@", error);
            };
            
            printController.printFormatter = printFormatter;
            printController.printInfo = printInfo;
            [printController presentAnimated:YES completionHandler:completionHandler];
        }
    }
    else
    {
        [Utility displaySimpleAlert:@"Print" withMessage:@"Unable to print this document"];
    }
         

}

- (IBAction)outputTestTwoAction:(id)sender 
{
    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *sampleFileURL = [bundle URLForResource:@"WordExport" withExtension:@"htm"];
//    NSString *filePath = [bundle pathForResource:@"WordExport" ofType:@"htm"];

    
//    NSError *error;
    NSString *htmlText = [NSString stringWithContentsOfURL:sampleFileURL encoding:NSUTF8StringEncoding error:NULL];
    
//    [self.webView loadRequest:[NSURLRequest requestWithURL:sampleFileURL]];
    
    UIPrintInfo *printInfo = [UIPrintInfo printInfo];
    
    printInfo.jobName = @"Test Output Two";
    printInfo.outputType = UIPrintInfoOutputGeneral;
    
    if([UIPrintInteractionController isPrintingAvailable])
    {
        Class printControllerClass = NSClassFromString(@"UIPrintInteractionController");
        if (printControllerClass) 
        {
            printController = [printControllerClass sharedPrintController];
            
            UIMarkupTextPrintFormatter *printFormatter = [[UIMarkupTextPrintFormatter alloc] initWithMarkupText:htmlText];
            
            void (^completionHandler)(UIPrintInteractionController *, BOOL, NSError *) =
            ^(UIPrintInteractionController *pic, BOOL completed, NSError *error) 
            {
                if (!completed && error) DLog(@"Print error: %@", error);
            };
            
            printController.printFormatter = printFormatter;
            
            printController.printInfo = printInfo;
            [printController presentAnimated:YES completionHandler:completionHandler];
        }
    }
    else
    {
        [Utility displaySimpleAlert:@"Print" withMessage:@"Unable to print this document"];
    }


}
- (void)dealloc {
    [webView release];
    [webView release];
    [super dealloc];
}
@end
