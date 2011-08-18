//
//  EditorView.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorView.h"

@interface EditorView()
{
@private
    BOOL keyboardIsShown;
}

- (void) moveTextViewForKeyboard:(NSNotification*)theNotification show:(BOOL)show;

@end

@implementation EditorView
@synthesize orderedListButton;
@synthesize boldButton;
@synthesize unorderedListButton;
@synthesize saveButton;
@synthesize webView;

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - Formatting
-(void)bold:(id)sender 
{
	[self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('bold')"];
}

-(void)orderedList:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('insertOrderedList')"];
}

-(void)unorderedList:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('insertUnorderedList')"];
}

#pragma mark - Keyboard Handler

// http://stackoverflow.com/questions/2807339/uikeyboardboundsuserinfokey-is-deprecated-what-to-use-instead

- (void)keyboardWillShow:(NSNotification *)theNotification 
{
    [self moveTextViewForKeyboard:theNotification show:YES];
}

- (void)keyboardWillHide:(NSNotification *)theNotification 
{
    [self moveTextViewForKeyboard:theNotification show:NO]; 
}

- (void) moveTextViewForKeyboard:(NSNotification*)theNotification show:(BOOL)show
{
    if (show && keyboardIsShown) {
        return;
    }
    
    NSDictionary* userInfo = [theNotification userInfo];
    
    // Get animation info from userInfo
    NSTimeInterval animationDuration;
    UIViewAnimationCurve animationCurve;
    
    CGRect keyboardEndFrame;
    
    [[userInfo objectForKey:UIKeyboardAnimationCurveUserInfoKey] getValue:&animationCurve];
    [[userInfo objectForKey:UIKeyboardAnimationDurationUserInfoKey] getValue:&animationDuration];
    
    
    [[userInfo objectForKey:UIKeyboardFrameEndUserInfoKey] getValue:&keyboardEndFrame];
    
    // Animate up or down
    [UIView beginAnimations:nil context:nil];
    [UIView setAnimationDuration:animationDuration];
    [UIView setAnimationCurve:animationCurve];
    
    CGRect newFrame = self.webView.frame;
    CGRect keyboardFrame = [self.view convertRect:keyboardEndFrame toView:nil];
    
    newFrame.size.height -= keyboardFrame.size.height * (show? 1 : -1);
    self.webView.frame = newFrame;
    
    keyboardIsShown = show;
    
    [UIView commitAnimations];
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *sampleFileURL = [bundle URLForResource:@"sample" withExtension:@"html"];
    [self.webView loadRequest:[NSURLRequest requestWithURL:sampleFileURL]];
    
    
    // register for keyboard notifications
    [[NSNotificationCenter defaultCenter] addObserver:self 
                                             selector:@selector(keyboardWillShow:) 
                                                 name:UIKeyboardWillShowNotification 
                                               object:self.view.window];
    // register for keyboard notifications
    [[NSNotificationCenter defaultCenter] addObserver:self 
                                             selector:@selector(keyboardWillHide:) 
                                                 name:UIKeyboardWillHideNotification 
                                               object:self.view.window];
    keyboardIsShown = NO;
}

- (void)viewDidUnload
{    
    // unregister for keyboard notifications while not visible.
    [[NSNotificationCenter defaultCenter] removeObserver:self 
                                                    name:UIKeyboardWillShowNotification 
                                                  object:nil]; 
    // unregister for keyboard notifications while not visible.
    [[NSNotificationCenter defaultCenter] removeObserver:self 
                                                    name:UIKeyboardWillHideNotification 
                                                  object:nil];  
    
    [self setWebView:nil];
    [self setBoldButton:nil];
    [self setOrderedListButton:nil];
    [self setUnorderedListButton:nil];
    [self setSaveButton:nil];
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

- (IBAction)orderedListAction:(id)sender 
{
    [self orderedList:sender];
}

- (IBAction)unorderedListAction:(id)sender 
{
    [self unorderedList:sender];
}

- (IBAction)saveAction:(id)sender {
}
@end
