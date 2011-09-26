//
//  EditorView.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorView.h"
#import "BDLinkedNote.h"

@interface EditorView()

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
        self.editorDocument = [BDLinkedNote retrieveWithUUID:theUuid];
    }
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - Formatting methods
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

-(void)subscript:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('subscript')"];
}

-(void)superscript:(id)sender 
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('superscript')"];    
}

-(void)insertHTML:(id)sender
{
    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand('insertHTML')"];
}

#pragma mark - Keyboard Handler

- (void)keyboardWillShow:(NSNotification *)theNotification 
{
    //NSLog(@"Show: %@", theNotification);
    
    [self moveTextViewForKeyboard:theNotification show:YES];
}

- (void)keyboardWillHide:(NSNotification *)theNotification 
{
    //NSLog(@"Hide: %@", theNotification);

    [self moveTextViewForKeyboard:theNotification show:NO]; 
}

- (void)keyboardDidChangeFrame:(NSNotification *)theNotification
{
   // NSLog(@"KDC: %@", theNotification);
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
    
    
    UIMenuController *controller = [UIMenuController sharedMenuController];
    controller.menuItems = [self generateMenuItems];

}

- (void)viewDidUnload
{    

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

#pragma mark - Button Actions
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
        self.editorDocument = [BDLinkedNote retrieveWithUUID:[BDLinkedNote create]];
    }
    self.editorDocument.modifiedDate = [NSDate date];

    self.editorDocument.documentText = [webView stringByEvaluatingJavaScriptFromString:@"document.documentElement.outerHTML"];
    
    NSLog(@"Document HTML = [%@]", self.editorDocument.documentText);
    
    [[DataController sharedInstance] saveContext];
}

- (IBAction)settingsToggle:(id)sender 
{
    
}

#pragma mark - Instance methods

-(void)insertSymbol {
    // do fancy stuff here...figure out the symbol, then call insertHTML
    NSString *beta = @"&#946";
    NSString *execCommand = [NSString stringWithFormat:@"document.execCommand(\'insertHTML\', false, \'%@\')",beta];
    NSLog(@"html string: %@", execCommand);
    [self.webView stringByEvaluatingJavaScriptFromString:execCommand];
    NSLog(@"Document HTML = [%@]", [webView stringByEvaluatingJavaScriptFromString:@"document.documentElement.outerHTML"]);

//    [self.webView stringByEvaluatingJavaScriptFromString:@"document.execCommand(\'insertHTML\',false, \'&#946\')"];
//    NSLog(@"Document HTML = [%@]", [webView stringByEvaluatingJavaScriptFromString:@"document.documentElement.outerHTML"]);

}

-(NSArray *)generateMenuItems {
    NSMutableArray *tmpArray = [[NSMutableArray alloc] initWithCapacity:0];
    
    UIMenuItem *miBold = [[UIMenuItem alloc] initWithTitle:@"Bold" action:@selector(bold:)];
    [tmpArray addObject:miBold];
    
    UIMenuItem *miSymbol = [[UIMenuItem alloc] initWithTitle:@"\u03B2" action:@selector(insertSymbol)];
    [tmpArray addObject:miSymbol];
    
    UIMenuItem *miSubscript = [[UIMenuItem alloc] initWithTitle:@"Subscript" action:@selector(subscript:)];
    [tmpArray addObject:miSubscript];
    
    UIMenuItem *miSuperscript = [[UIMenuItem alloc] initWithTitle:@"Superscript" action:@selector(superscript:)];
    [tmpArray addObject:miSuperscript];
    
    return [NSArray arrayWithArray:tmpArray];
}

@end
