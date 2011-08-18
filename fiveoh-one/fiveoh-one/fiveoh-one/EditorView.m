//
//  EditorView.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorView.h"

@implementation EditorView
@synthesize boldButton;
@synthesize webView;

/*
- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}
*/
- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - Formatting
-(void)bold:(id)sender 
{
	[self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('Bold')"];
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *sampleFileURL = [bundle URLForResource:@"sample" withExtension:@"html"];
    [self.webView loadRequest:[NSURLRequest requestWithURL:sampleFileURL]];
}

- (void)viewDidUnload
{
    [self setWebView:nil];
    [self setBoldButton:nil];
    [super viewDidUnload];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
	return YES;
}
- (IBAction)boldAction:(id)sender 
{
    [self bold:sender];
}
@end
