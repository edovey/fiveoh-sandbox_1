//
//  PresentationView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "PresentationView.h"
#import "BDPresentation.h"

@implementation PresentationView
@synthesize parentId;
@synthesize dataTableView;
@synthesize presentationArray;


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
    self.title = @"Presentations";
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.presentationArray = [NSArray arrayWithArray:[BDPresentation retrieveAllWithParentUUID:parentId]];
}

- (void)viewDidUnload
{
    [parentId release];
    [self setDataTableView:nil];
    [dataTableView release];
    dataTableView = nil;
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
    //TODO:
    // Show the disclosure indicator if the section has categories
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    /*
     TherapyView *vwTherapy = [[TherapyView alloc] initWithNibName:@"TherapyView" bundle:nil];
     vwTherapy.parentId = [self.presentationArray objectAtIndex:indexPath.row] uuid]];
     [self.navigationController pushViewController:vwTherapy animated:YES];
     [vwTherapy release];
     */
}

@end

