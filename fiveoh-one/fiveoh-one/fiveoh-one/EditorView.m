//
//  EditorView.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorView.h"
#import "EditorDocument.h"

@interface EditorView()
{
@private
    BOOL keyboardIsShown;
}

- (void) moveTextViewForKeyboard:(NSNotification*)theNotification show:(BOOL)show;

@end

@implementation EditorView

@synthesize editorDocument;

@synthesize orderedListButton;
@synthesize boldButton;
@synthesize unorderedListButton;
@synthesize indentButton;
@synthesize outdentButton;
@synthesize saveButton;
@synthesize settingsSwitch;
@synthesize webView;

- (void)assignDocumentUuid:(NSString *)theUuid
{
    if (nil != theUuid)
    {
        self.editorDocument = [EditorDocument retrieveWithUUID:theUuid];
    }
}

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

-(void)indent:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('indent')"];
}

-(void)outdent:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('outdent')"];
}

#pragma mark - Keyboard Handler

- (void)keyboardWillShow:(NSNotification *)theNotification 
{
    NSLog(@"Show: %@", theNotification);
    
    [self moveTextViewForKeyboard:theNotification show:YES];
}

- (void)keyboardWillHide:(NSNotification *)theNotification 
{
    NSLog(@"Hide: %@", theNotification);

    [self moveTextViewForKeyboard:theNotification show:NO]; 
}

- (void)keyboardDidChangeFrame:(NSNotification *)theNotification
{
    NSLog(@"KDC: %@", theNotification);
}

- (void) moveTextViewForKeyboard:(NSNotification*)theNotification show:(BOOL)show
{
    if(!self.settingsSwitch.on)
    {
        return;
    }
    
    if (show && keyboardIsShown) 
    {
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
    
    if (nil != self.editorDocument)
    {
        [self.webView loadHTMLString:self.editorDocument.documentText baseURL:[NSURL URLWithString:@""]];
    }
    else
    {
        NSBundle *bundle = [NSBundle mainBundle];
        NSURL *sampleFileURL = [bundle URLForResource:@"sample" withExtension:@"html"];
        [self.webView loadRequest:[NSURLRequest requestWithURL:sampleFileURL]];
    }
    
    // register for keyboard notifications
    [[NSNotificationCenter defaultCenter] addObserver:self 
                                             selector:@selector(keyboardWillShow:) 
                                                 name:UIKeyboardWillShowNotification 
                                               object:self.view.window];
    
    [[NSNotificationCenter defaultCenter] addObserver:self 
                                             selector:@selector(keyboardDidChangeFrame:) 
                                                 name:UIKeyboardDidChangeFrameNotification 
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
    
    [[NSNotificationCenter defaultCenter] removeObserver:self 
                                                    name:UIKeyboardDidChangeFrameNotification 
                                                  object:nil];  

    [self setWebView:nil];
    [self setBoldButton:nil];
    [self setOrderedListButton:nil];
    [self setUnorderedListButton:nil];
    [self setSaveButton:nil];
    [self setIndentButton:nil];
    [self setOutdentButton:nil];
    [self setSettingsSwitch:nil];
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

- (IBAction)indentAction:(id)sender 
{
    [self indent:sender];
}

- (IBAction)outdentAction:(id)sender 
{
    [self outdent:sender];
}

- (IBAction)saveAction:(id)sender 
{
    if(nil == self.editorDocument)
    {
        self.editorDocument = [EditorDocument create];
    }
    self.editorDocument.modifiedDate = [NSDate date];

    self.editorDocument.documentText = [webView stringByEvaluatingJavaScriptFromString:@"document.documentElement.outerHTML"];
    
    [[DataController sharedInstance] saveContext];
}

- (IBAction)settingsToggle:(id)sender 
{
    
}

@end
