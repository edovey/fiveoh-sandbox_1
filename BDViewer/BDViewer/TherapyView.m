//
//  TherapyView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "TherapyView.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"

@interface TherapyView() 
-(void)retrieveOverviewForTherapy;
-(void)buildHTMLFromData;
-(void)loadHTMLIntoWebView;
@end

@implementation TherapyView
@synthesize dataWebView;
@synthesize therapyArray;
@synthesize parentId;
@synthesize diseaseId;
@synthesize overviewHTMLString;

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
    self.title = @"Therapies";
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.therapyArray = [NSArray arrayWithArray:[BDTherapyGroup retrieveAllWithParentUUID:parentId]];
    [self retrieveOverviewForTherapy];
    [self buildHTMLFromData];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    [dataWebView release];
    dataWebView = nil;
    overviewHTMLString = nil;
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)dealloc {
    [parentId release];
    [diseaseId release];
    [therapyArray release];
    [dataWebView release];
    [overviewHTMLString release];
    [super dealloc];
}

#pragma mark - Private Class methods
-(void)retrieveOverviewForTherapy
{
    overviewHTMLString = @"Overview Text";
    
}

-(void)buildHTMLFromData
{
    
}

-(void)loadHTMLIntoWebView 
{
    [self.dataWebView loadHTMLString:[NSString stringWithFormat:@"<html><body><font face='Helvetica' size='3.0'>%@<br></font></body></html>",self.overviewHTMLString] baseURL:[NSURL URLWithString:@""]];
    [self.dataWebView setBackgroundColor:[UIColor clearColor]];
    [self.dataWebView setOpaque:NO];
}

@end
