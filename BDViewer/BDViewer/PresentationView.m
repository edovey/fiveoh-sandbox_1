//
//  PresentationView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "PresentationView.h"
#import "BDPresentation.h"
#import "TherapyView.h"

@interface PresentationView() 
-(void)retrieveOverviewForPresentation;
-(void)buildHTMLFromData;
-(void)loadHTMLIntoWebView;
@end

@implementation PresentationView
@synthesize parentId;
@synthesize parentName;
@synthesize dataTableView;
@synthesize dataWebView;
@synthesize presentationArray;
@synthesize overviewHTMLString;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName
{
    self = [super initWithNibName:@"PresentationView" bundle:nil];
    if(self)
    {
        parentId = [pParentId retain];
        parentName = [pParentName retain];
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
    self.title = parentName;
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.presentationArray = [NSArray arrayWithArray:[BDPresentation retrieveAllWithParentUUID:parentId]];
    [self retrieveOverviewForPresentation];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    parentId = nil;
    parentName = nil;
    [self setDataTableView:nil];
    [dataTableView release];
    dataTableView = nil;
    [self setDataWebView:nil];
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
    [dataTableView release];
    [presentationArray release];
    [parentId release];
    [parentName release];
    [dataWebView release];
    [overviewHTMLString release];
    [super dealloc];
}

#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.presentationArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[self.presentationArray objectAtIndex:indexPath.row] name ];
    cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    BDPresentation *presentation = [presentationArray objectAtIndex:indexPath.row];
    TherapyView *vwTherapy = [[TherapyView alloc] initWithPresentationId:presentation.uuid withPresentationName:presentation.name];
    [self.navigationController pushViewController:vwTherapy animated:YES];
    [vwTherapy release];
}

#pragma mark - Private Class methods
-(void)retrieveOverviewForPresentation
{
    overviewHTMLString = @"Overview Text";
    //TODO:  retrieve text of overview.  if none,hide control and reset origin of tableview.
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

