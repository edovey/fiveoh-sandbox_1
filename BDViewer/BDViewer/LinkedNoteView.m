//
//  LinkedNoteView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-11-01.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "LinkedNoteView.h"
#import "BDLinkedNote.h"

@interface LinkedNoteView()
-(void)loadHTMLIntoWebView; 

@end

@implementation LinkedNoteView
@synthesize linkedNoteId;
@synthesize dataWebView;
@synthesize detailHTMLString;

-(id)initWithLinkId:(NSString *)pLinkId
{
    self = [super initWithNibName:@"LinkedNoteView" bundle:nil];
    if(self)
    {
        self.linkedNoteId = [pLinkId retain];
        self.title = @"Comments";
    }
    return self;
}

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
    // Do any additional setup after loading the view from its nib.
    self.dataWebView.delegate = self;
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    [self setDataWebView:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
    self.linkedNoteId = nil;
    self.dataWebView = nil;
    self.detailHTMLString = nil;
}

-(void)dealloc
{
    [dataWebView release];
    [linkedNoteId release];
    [detailHTMLString release];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark Private Methods
-(void)loadHTMLIntoWebView 
{
    NSURL *bundleURL = [[NSBundle mainBundle] bundleURL];
    
    BDLinkedNote *note = [BDLinkedNote retrieveWithUUID:linkedNoteId];
    self.detailHTMLString = [NSString stringWithFormat:@"%@",note.documentText];

    [self.dataWebView loadHTMLString:[NSString stringWithFormat:@"<html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"bdviewer.css\" /> </head><body>%@</body></html>",self.detailHTMLString] baseURL:bundleURL];
    [self.dataWebView setBackgroundColor:[UIColor clearColor]];
    [self.dataWebView setOpaque:NO];
}

@end
